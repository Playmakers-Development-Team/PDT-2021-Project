using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Units;

namespace Managers
{
    public delegate void CommandAction(Command cmd);
    
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
        private readonly Dictionary<Type, LinkedList<CommandAction>> executeListeners =
            new Dictionary<Type, LinkedList<CommandAction>>();

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
            
            if (executeListeners.ContainsKey(commandType))
            {
                foreach (var action in executeListeners[commandType])
                {
                    action.Invoke(command);
                }
            }
        }

        /// <summary>
        /// Listen to when the command is executed
        ///
        /// <example>
        /// How to use.
        /// <code>
        ///     ListenExecuteCommand&lt;EndTurnCommand&gt;((cmd) => Debug.Log("Unit turn has ended!"));
        /// </code>
        /// </example>
        /// </summary>
        public void ListenCommand<T>(CommandAction action) where T : Command
        {
            Type commandType = typeof(T);
            
            if (!executeListeners.ContainsKey(commandType))
            {
                executeListeners[commandType] = new LinkedList<CommandAction>();
            }

            executeListeners[commandType].AddLast(action);
        }

        /// <summary>
        /// Listen to when the command is executed. We need to remove the CommandAction after object
        /// is destroyed to prevent errors and memory leaks. E.g for not having the action be called
        /// after some GameObject is no longer in use.
        ///
        /// <example>
        /// Keep the action stored as a variable somewhere, then remove it when done with it.
        /// <code>
        ///     CommandAction action = (cmd) =>
        ///     {
        ///         Debug.Log("unit has ended their turn!");
        ///     }
        ///     ListenExecuteCommand&lt;EndTurnCommand&gt;(action);
        ///     ...
        ///     RemoveExecuteListener&lt;EndTurnCommand&gt;(action);
        /// </code>
        /// </example>
        /// </summary>
        public void UnlistenCommand<T>(CommandAction action) where T : Command
        {
            Type commandType = typeof(T);
            var foundContext = executeListeners[commandType]
                .FirstOrDefault(a => a == action);

            executeListeners[commandType].Remove(foundContext);

            if (executeListeners[commandType].Count == 0)
            {
                executeListeners.Remove(commandType);
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