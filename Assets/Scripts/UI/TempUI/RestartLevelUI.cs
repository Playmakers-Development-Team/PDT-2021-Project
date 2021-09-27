using System;
using Commands;
using Game;
using Game.Commands;
using Managers;
using UnityEngine;

namespace UI.TempUI
{
    public class RestartLevelUI : MonoBehaviour
    {
        private CommandManager commandManager;
        
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        public void RestartLevel()
        {
            commandManager.ExecuteCommand(new RestartEncounterCommand());
        }
    }
}
