using System.Collections;
using System.Collections.Generic;
using Commands;
using Managers;
using Units;
using UnityEngine;

namespace UI
{
    
    public class EndTurnUI : MonoBehaviour
    {

        private CommandManager commandManager;
        private TurnManager turnManager;

        private void Start()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        public void EndTurn()
        {
            commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
        }

    }

}
