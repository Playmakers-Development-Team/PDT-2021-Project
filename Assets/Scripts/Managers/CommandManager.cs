using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class CommandManager : Manager
    { 
        // TODO: The Command system should not depend of the Unit system's HistoricalCommand.
        // TODO: CommandHistory should be moved to the Unit system.
        /// <summary>
        /// A record of all the historical commands, the first command in the list is the latest command.
        /// </summary>
        private readonly LinkedList<HistoricalCommand> commandHistory = new LinkedList<HistoricalCommand>();

        /// <summary>
        /// Keeps track of which listeners need to be called when a command is executed.
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
        /// Stores every command by name. Useful for <see cref="CommandListener"/>.
        /// </summary>
        private readonly Dictionary<string, Type> commandTypes = new Dictionary<string, Type>();
        
        /// <summary>
        /// Used for the logging and debugging. Makes it easy to decouple functionality from editor
        /// only scripts.
        /// </summary>
        public event Action<Command> OnCommandExecuteEvent;

        public override void ManagerStart()
        {
            var allCommandTypes = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(a => a.GetTypes()).Where(t => typeof(Command).IsAssignableFrom(t));

            foreach (Type commandType in allCommandTypes)
            {
                commandTypes[commandType.Name] = commandType;
            }
        }

        /// <summary>
        /// Checks if a command with a given class name exists.
        /// </summary>
        /// <param name="name">Command class name e.g "EndTurnCommand"</param>
        public bool HasCommandType(string name) => commandTypes.ContainsKey(name);
        
        /// <summary>
        /// Gets a command by class name.
        /// </summary>
        /// <param name="name">Command class name e.g "EndTurnCommand"</param>
        public Type GetCommandType(string name) =>
            commandTypes.ContainsKey(name) ? commandTypes[name] : null;

        /// <summary>
        /// Execute the given command. Anything listening to this type of command would be notified.
        ///
        /// <p> If the command is a <c>HistoricalCommand</c>, it will be added to the command
        /// history, so it will be possible to undo the command.</p>
        /// </summary>
        public void ExecuteCommand(Command command)
        {
            Type commandType = command.GetType();

            if (command is HistoricalCommand historicalCommand)
            {
                commandHistory.AddFirst(historicalCommand);
            }
            
            command.Execute();
            OnCommandExecuteEvent?.Invoke(command);
            
            if (listeners.ContainsKey(commandType))
            {
                // Store actions as an array to prevent array modification exceptions
                var actions = listeners[commandType].ToArray();
                
                foreach (var action in actions)
                {
                    // Check if the action is a catch listener
                    if (caughtCommands.ContainsKey(action))
                    {
                        // If there is already a command with the current command type, remove it
                        caughtCommands[action].RemoveAll(c => c.GetType() == commandType);
                        
                        // The current command type is now fulfilled for that catch listener
                        caughtCommands[action].Add(command);
                        
                        var requiredCommandTypes = action.GetType().GetGenericArguments();
                        var caughtCommandTypes = caughtCommands[action].Select(cmd => cmd.GetType());
                        
                        // If any required commands are not in the caught commands, the
                        // requirements have not been met
                        bool isNotReady = requiredCommandTypes.Except(caughtCommandTypes).Any();

                        if (!isNotReady)
                        {
                            // Need to cast to Array<object> for it to pass in parameters properly
                            action.DynamicInvoke(caughtCommands[action].ToArray<object>());
                            
                            // Remove it afterwards
                            RemoveCatchListener(action);
                        }
                    }
                    // Otherwise, try to invoke it with the command parameter
                    else if (action.GetMethodInfo().GetParameters().Any())
                    {
                        action.DynamicInvoke(command);
                    }
                    // Otherwise just invoke it as if it had no parameters, listeners may simply be an Action
                    // This might be the case for CommandListener where it only requires a simple Action
                    else
                    {
                        action.DynamicInvoke();
                    }
                }
            }
        }

        /// <summary>
        /// Listen to when the command is executed. Do something every time the command is executed.
        ///
        /// <example>
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
        /// Listen to when the command is executed. Do something every time the command is executed.
        /// This version of the function takes in a simple type to do something simple
        ///
        /// <example>
        /// <code>
        ///     ListenCommand(typeof(EndTurnCommand), () => Debug.Log("Unit turn has ended!"));
        /// </code>
        /// </example>
        /// </summary>
        public void ListenCommand(Type type, Action action)
        {
            if (!typeof(Command).IsAssignableFrom(type))
                throw new ArgumentException($"Expected {nameof(Command)} type, but got {type}");
            
            RegisterListener(type, action);
        }

        /// <summary>
        /// Stop listening to a command execution. We need to remove the Action object when we 
        /// don't need it to respond to command execution anymore. E.g We need to remove the
        /// Action object after some GameObject is no longer in use. Also to prevent errors and
        /// memory leaks.
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
        /// Stop listening to a command execution. We need to remove the Action object when we 
        /// don't need it to respond to command execution anymore. E.g We need to remove the
        /// Action object after some GameObject is no longer in use. Also to prevent errors and
        /// memory leaks.
        /// <br/>
        /// This version of the function takes in a type for simple actions.
        ///
        /// </summary>
        public void UnlistenCommand(Type type, Action action)
        {
            if (!type.IsAssignableFrom(typeof(Command)))
                throw new ArgumentException($"Expected {nameof(Command)} type, but got {type}");

            var foundListener = listeners[type]
                .FirstOrDefault(a => a == (Delegate) action);

            // Check if the delegate was actually added in the list in the first place
            if (foundListener != null)
                RemoveListener(type, foundListener);
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

        /// <summary>
        /// <p>Wait for a command to be executed using async/await.</p>
        /// <p>Commands can be filtered so that we can wait for a command that we exactly want.
        /// If multiple commands are to be caught, then wait for all of them.</p>
        ///
        /// <example>
        /// How to use. E.g wait for all players to be ready.
        /// <code>
        ///     PlayerUnitsReadyCommand cmd = await WaitForCommand&lt;PlayerUnitsReadyCommand&gt;();
        /// </code>
        /// How to use. E.g wait for a player unit to start moving and finish moving.
        /// <code>
        ///     var (startMoveCmd, endMoveCmd) = await WaitForCommand&lt;StartMoveCommand, EndMoveCommand&gt;();
        /// </code>
        /// How to use but check if the command returns the expected parameters.
        /// E.g wait for a player unit to start moving.
        /// <code>
        ///     StartMoveCommand cmd = await WaitForCommand&lt;StartMoveCommand&gt;((unit) => unit is PlayerUnit);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="filter">
        /// A lambda function which should return true if this is the command that we are expecting
        /// and hence we should stop waiting. Can be left empty if we accept any command.
        /// </param>
        public async Task<T> WaitForCommand<T>(Predicate<T> filter = null) where T : Command
        {
            T caughtCmd1 = null;
            
            bool hasCaught = false;
            CatchCommand((T cmd1) =>
            {
                caughtCmd1 = cmd1;
                
                if (filter != null && filter(cmd1))
                    hasCaught = true;
            });
            await UniTask.WaitUntil(() => hasCaught);
            return caughtCmd1;
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed.</p>
        /// Please see <see cref="WaitForCommand{T}"/> for detailed information and examples.
        /// </summary>
        public async Task<(T1, T2)> WaitForCommand<T1, T2>(Func<T1, T2, bool> filter = null) 
            where T1 : Command
            where T2 : Command
        {
            T1 caughtCmd1 = null;
            T2 caughtCmd2 = null;
            
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2) =>
            {
                caughtCmd1 = cmd1;
                caughtCmd2 = cmd2;
                
                if (filter != null && filter(cmd1, cmd2))
                    hasCaught = true;
            });
            await UniTask.WaitUntil(() => hasCaught);
            return (caughtCmd1, caughtCmd2);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed.</p>
        /// Please see <see cref="WaitForCommand{T}"/> for detailed information and examples.
        /// </summary>
        public async Task<(T1, T2, T3)> WaitForCommand<T1, T2, T3>(Func<T1, T2, T3, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
        {
            T1 caughtCmd1 = null;
            T2 caughtCmd2 = null;
            T3 caughtCmd3 = null;
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3) =>
            {
                caughtCmd1 = cmd1;
                caughtCmd2 = cmd2;
                caughtCmd3 = cmd3;
                
                if (filter != null && filter(cmd1, cmd2, cmd3))
                    hasCaught = true;
            });
            await UniTask.WaitUntil(() => hasCaught);
            return (caughtCmd1, caughtCmd2, caughtCmd3);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed.</p>
        /// Please see <see cref="WaitForCommand{T}"/> for detailed information and examples.
        /// </summary>
        public async Task<(T1, T2, T3, T4)> WaitForCommand<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
        {
            T1 caughtCmd1 = null;
            T2 caughtCmd2 = null;
            T3 caughtCmd3 = null;
            T4 caughtCmd4 = null;
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3, T4 cmd4) =>
            {
                caughtCmd1 = cmd1;
                caughtCmd2 = cmd2;
                caughtCmd3 = cmd3;
                caughtCmd4 = cmd4;
                
                if (filter != null && filter(cmd1, cmd2, cmd3, cmd4))
                    hasCaught = true;
            });
            await UniTask.WaitUntil(() => hasCaught);
            return (caughtCmd1, caughtCmd2, caughtCmd3, caughtCmd4);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed.</p>
        /// Please see <see cref="WaitForCommand{T}"/> for detailed information and examples.
        /// </summary>
        public async Task<(T1, T2, T3, T4, T5)> WaitForCommand<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
            where T5 : Command
        {
            T1 caughtCmd1 = null;
            T2 caughtCmd2 = null;
            T3 caughtCmd3 = null;
            T4 caughtCmd4 = null;
            T5 caughtCmd5 = null;
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3, T4 cmd4, T5 cmd5) =>
            {
                caughtCmd1 = cmd1;
                caughtCmd2 = cmd2;
                caughtCmd3 = cmd3;
                caughtCmd4 = cmd4;
                caughtCmd5 = cmd5;
                
                if (filter != null && filter(cmd1, cmd2, cmd3, cmd4, cmd5))
                    hasCaught = true;
            });
            await UniTask.WaitUntil(() => hasCaught);
            return (caughtCmd1, caughtCmd2, caughtCmd3, caughtCmd4, caughtCmd5);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed in a Unity Coroutine.</p>
        /// <p>Commands can be filtered so that we can wait for a command that we exactly want.
        /// If multiple commands are to be caught, then wait for all of them.</p>
        ///
        /// <example>
        /// How to use. E.g wait for all players to be ready.
        /// <code>
        ///     yield return WaitForCommandYield&lt;PlayerUnitsReadyCommand&gt;();
        /// </code>
        /// How to use but check if the command returns the expected parameters.
        /// E.g wait for a player unit to start moving.
        /// <code>
        ///     yield return WaitForCommandYield&lt;StartMoveCommand&gt;((unit) => unit is PlayerUnit);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="filter">
        /// A lambda function which should return true if this is the command that we are expecting
        /// and hence we should stop waiting. Can be left empty if we accept any command.
        /// </param>
        public IEnumerator WaitForCommandYield<T>(Predicate<T> filter = null) where T : Command
        {
            bool hasCaught = false;
            CatchCommand((T cmd1) =>
            {
                if (filter != null && filter(cmd1))
                    hasCaught = true;
            });
            yield return new WaitUntil(() => hasCaught);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed in a Unity Coroutine.</p>
        /// Please see <see cref="WaitForCommandYieldYield{T}"/> for detailed information and examples.
        /// </summary>
        public IEnumerator WaitForCommandYield<T1, T2>(Func<T1, T2, bool> filter = null) 
            where T1 : Command
            where T2 : Command
        {
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2) =>
            {
                if (filter != null && filter(cmd1, cmd2))
                    hasCaught = true;
            });
            yield return new WaitUntil(() => hasCaught);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed in a Unity Coroutine.</p>
        /// Please see <see cref="WaitForCommandYieldYield{T}"/> for detailed information and examples.
        /// </summary>
        public IEnumerator WaitForCommandYield<T1, T2, T3>(Func<T1, T2, T3, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
        {
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3) =>
            {
                if (filter != null && filter(cmd1, cmd2, cmd3))
                    hasCaught = true;
            });
            yield return new WaitUntil(() => hasCaught);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed in a Unity Coroutine.</p>
        /// Please see <see cref="WaitForCommandYieldYield{T}"/> for detailed information and examples.
        /// </summary>
        public IEnumerator WaitForCommandYield<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
        {
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3, T4 cmd4) =>
            {
                if (filter != null && filter(cmd1, cmd2, cmd3, cmd4))
                    hasCaught = true;
            });
            yield return new WaitUntil(() => hasCaught);
        }
        
        /// <summary>
        /// <p>Wait for a command to be executed in a Unity Coroutine.</p>
        /// Please see <see cref="WaitForCommandYieldYield{T}"/> for detailed information and examples.
        /// </summary>
        public IEnumerator WaitForCommandYield<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, bool> filter = null) 
            where T1 : Command
            where T2 : Command
            where T3 : Command
            where T4 : Command
            where T5 : Command
        {
            bool hasCaught = false;
            CatchCommand((T1 cmd1, T2 cmd2, T3 cmd3, T4 cmd4, T5 cmd5) =>
            {
                if (filter != null && filter(cmd1, cmd2, cmd3, cmd4, cmd5))
                    hasCaught = true;
            });
            yield return new WaitUntil(() => hasCaught);
        }

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