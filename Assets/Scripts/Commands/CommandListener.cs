using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Commands
{
    public class CommandListener : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private string commandTypeName;
        public UnityEvent OnCommandExecute;

        private CommandManager commandManager;
        
        public Type CommandType { get; private set; }
        

        private void OnEnable()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            CommandType = commandManager.GetCommandType(commandTypeName);

            if (CommandType == null)
            {
                Debug.LogError($"Command Listener listening to invalid type \"{CommandType}\"");
            }
            else if (!typeof(Command).IsAssignableFrom(CommandType))
            {
                Debug.LogError($"Command Listener can only listen to {nameof(Command)} types, got type {CommandType}");
            }
            else
            {
                commandManager.ListenCommand(CommandType, OnExecuteCommand);
            }
        }

        private void OnExecuteCommand()
        {
            OnCommandExecute?.Invoke();
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand(CommandType, OnExecuteCommand);
        }
    }
}
