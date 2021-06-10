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
        private List<Command> commandHistory = new List<Command>();

        private readonly Dictionary<Type, LinkedList<CommandAction>> executeListeners =
            new Dictionary<Type, LinkedList<CommandAction>>();

        // private List<Command> turnOrder = new List<Command>();

        private int currentCommandHistoryIndex;

        public void ExecuteCommand(Command command)
        {
            Type commandType = command.GetType();
            commandHistory.Add(command);
            command.Execute();
            
            if (executeListeners.ContainsKey(commandType))
            {
                foreach (var action in executeListeners[commandType])
                {
                    action.Invoke(command);
                }
            }
            
            currentCommandHistoryIndex = commandHistory.Count - 1;
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
        public void ListenExecuteCommand<T>(CommandAction action) where T : Command
        {
            Type commandType = typeof(T);
            
            if (!executeListeners.ContainsKey(commandType))
            {
                executeListeners[commandType] = new LinkedList<CommandAction>();
            }

            executeListeners[commandType].AddLast(action);
        }

        /// <summary>
        /// Listen to when the command is executed. Remember to remove the CommandAction after object
        /// is destroyed to prevent errors and memory leaks.
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
        public void RemoveExecuteListener<T>(CommandAction action) where T : Command
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
    }
}