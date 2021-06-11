using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Units;

namespace Managers
{
    public class CommandManager : Manager
    { 
        /// <summary>
        /// A record of all the historical commands, the first command in the list is the latest command.
        /// </summary>
        private readonly LinkedList<HistoricalCommand> commandHistory = new LinkedList<HistoricalCommand>();

        /// <summary>
        /// Basically keeps tracks of what listeners need to be called when some command is executed.
        /// Stores all listeners in a graph data structure for efficiency.
        /// </summary>
        private readonly Dictionary<Type, LinkedList<Delegate>> listeners =
            new Dictionary<Type, LinkedList<Delegate>>();

        /// <summary>
        /// Used for catch listeners, keeps track of all commands that are executed after a given
        /// catch listener is registered.
        /// </summary>
        private readonly Dictionary<Delegate, List<Command>> caughtCommands =
            new Dictionary<Delegate, List<Command>>();

        /// <summary>
        /// Execute the given command. Anything listening to this type of command would be notified.
        ///
        /// <p> If the command is a <c>HistoricalCommand</c>, it will be added to the command history, so
        /// it will be possible to undo the command. </p>
        /// </summary>
        public void ExecuteCommand(Command command)
        {
            Type commandType = command.GetType();

            if (command is HistoricalCommand historicalCommand)
            {
                commandHistory.AddFirst(historicalCommand);
            }
            
            command.Execute();
            
            if (listeners.ContainsKey(commandType))
            {
                // We to remove listeners as our final step to prevent array modification exceptions
                var catchingListenersToRemove = new List<Delegate>();
                
                foreach (var action in listeners[commandType])
                {
                    // Check if the action is a catch listener
                    if (caughtCommands.ContainsKey(action))
                    {
                        // If there is already a command with the current command type, remove it
                        caughtCommands[action].RemoveAll(c => c.GetType() == commandType);
                        
                        // The current command type is now fulfilled for that catch listener
                        caughtCommands[action].Add(command);
                        
                        var requiredCommandTypes = action.GetType().GetGenericArguments();
                        
                        // Check if all the command requirements has been met
                        // Basically compare if 2 different lists are the same
                        bool isNotReady = caughtCommands[action]
                            .Select(cmd => cmd.GetType())
                            .Except(requiredCommandTypes)
                            .Any();

                        if (!isNotReady)
                        {
                            catchingListenersToRemove.Add(action);
                            // Need to cast to Array<object> for it to pass in parameters properly
                            action.DynamicInvoke(caughtCommands[action].ToArray<object>());
                        }
                    }
                    // Otherwise just invoke it normally
                    else
                    { 
                        action.DynamicInvoke(command);
                    }
                }
                
                catchingListenersToRemove.ForEach(RemoveCatchListener);
            }
        }

        /// <summary>
        /// Listen to when the command is executed. Do something every time the command is executed.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     ListenCommand&lt;EndTurnCommand&gt;((cmd) => Debug.Log("Unit turn has ended!"));
        /// </code>
        /// </example>
        /// </summary>
        public void ListenCommand<T>(Action<T> action) where T : Command
        {
            RegisterListener(typeof(T), action);
        }

        /// <summary>
        /// Stop listening to a command execution. We need to remove the Action object when we 
        /// don't need it to respond to command execution anymore. E.g We need to remove the
        /// Action object after some GameObject is no longer in use. Also to prevent errors and memory leaks
        ///
        /// <example>
        /// Keep the action stored as a variable somewhere, then remove it when done with it.
        /// <code>
        ///     Action&lt;EndTurnCommand&gt; action = (cmd) =>
        ///     {
        ///         Debug.Log("unit has ended their turn!");
        ///     }
        ///     ListenCommand&lt;EndTurnCommand&gt;(action);
        ///     ...
        ///     UnlistenCommand&lt;EndTurnCommand&gt;(action);
        /// </code>
        /// </example>
        /// </summary>
        public void UnlistenCommand<T>(Action<T> action) where T : Command
        {
            Type commandType = typeof(T);
            var foundListener = listeners[commandType]
                .FirstOrDefault(a => a == (Delegate) action);

            // Check if the delegate was actually added in the list in the first place
            if (foundListener != null)
                RemoveListener(commandType, foundListener);
        }

        /// <summary>
        /// Do something <b>only once</b>, when a command is executed. If multiple commands are
        /// to be caught, then wait for all of them.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     CatchCommand&lt;PlayerUnitsReadyCommand&gt;((cmd) => Debug.Log("Player units are ready!"));
        /// </code>
        /// </example>
        /// </summary>
        public void CatchCommand<T>(Action<T> action) where T : Command => RegisterCatchListener(action);

        /// <summary>
        /// Do something <b>only once</b>, when a command is executed. If multiple commands are
        /// to be caught, then wait for all of them.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     CatchCommand&lt;PlayerUnitsReadyCommand&gt;((cmd) => Debug.Log("Player units are ready!"));
        /// </code>
        /// </example>
        /// </summary>
        public void CatchCommand<T1, T2>(Action<T1, T2> action)
            where T1 : Command 
            where T2 : Command =>
            RegisterCatchListener(action);

        /// <summary>
        /// Do something <b>only once</b>, when a command is executed. If multiple commands are
        /// to be caught, then wait for all of them.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     CatchCommand&lt;PlayerUnitsReadyCommand&gt;((cmd) => Debug.Log("Player units are ready!"));
        /// </code>
        /// </example>
        /// </summary>
        public void CatchCommand<T1, T2, T3>(Action<T1, T2, T3> action)
            where T1 : Command 
            where T2 : Command 
            where T3 : Command =>
            RegisterCatchListener(action);

        /// <summary>
        /// Do something <b>only once</b>, when a command is executed. If multiple commands are
        /// to be caught, then wait for all of them.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     CatchCommand&lt;PlayerUnitsReadyCommand&gt;((cmd) => Debug.Log("Player units are ready!"));
        /// </code>
        /// </example>
        /// </summary>
        public void CatchCommand<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
            where T1 : Command 
            where T2 : Command 
            where T3 : Command 
            where T4 : Command =>
            RegisterCatchListener(action);

        /// <summary>
        /// Do something <b>only once</b>, when a command is executed. If multiple commands are
        /// to be caught, then wait for all of them.
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     CatchCommand&lt;PlayerUnitsReadyCommand&gt;((cmd) => Debug.Log("Player units are ready!"));
        /// </code>
        /// </example>
        /// </summary>
        public void CatchCommand<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
            where T5 : Command =>
            RegisterCatchListener(action);

        private void RegisterCatchListener(Delegate action)
        {
            if (caughtCommands.ContainsKey(action))
                return;
            
            Type[] commandTypes = action.GetType().GetGenericArguments();

            foreach (Type commandType in commandTypes)
            {
                RegisterListener(commandType, action);
            }
            
            caughtCommands.Add(action, new List<Command>());
        }

        private void RemoveCatchListener(Delegate action)
        {
            Type[] commandTypes = action.GetType().GetGenericArguments();

            foreach (Type commandType in commandTypes)
            {
                RemoveListener(commandType, action);
            }
            
            caughtCommands.Remove(action);
        }

        private void RegisterListener(Type commandType, Delegate action)
        {
            if (listeners.ContainsKey(commandType) && listeners[commandType].Contains(action))
                return;
            
            if (!listeners.ContainsKey(commandType))
                listeners[commandType] = new LinkedList<Delegate>();
            
            listeners[commandType].AddLast(action);
        }

        private void RemoveListener(Type commandType, Delegate action)
        {
            listeners[commandType].Remove(action);

            if (listeners[commandType].Count == 0)
            {
                listeners.Remove(commandType);
            }
        }

        /// <summary>
        /// Preforms Undo on the last historical command and removes it from the command history.
        /// </summary>
        public void UndoLastHistoricalCommand()
        {
            var historicalCommand = commandHistory.First.Value;
            commandHistory.RemoveFirst();
            historicalCommand.Undo();
        }

        /// <summary>
        /// Gets rid of all stored historical commands. That means all the commands that has undo
        /// are now forgotten.
        /// </summary>
        public void ClearCommandHistory()
        {
            commandHistory.Clear();
        }
    }
}