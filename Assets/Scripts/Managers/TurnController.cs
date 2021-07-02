using System.Collections.Generic;
using System.Linq;
using Commands;
using UI;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        /// <summary>
        /// A reference to the TurnManager.
        /// </summary>
        private TurnManager turnManager;

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();
            
            commandManager.CatchCommand<PlayerUnitsReadyCommand, EnemyUnitsReadyCommand>(
                (cmd1, cmd2) =>
                {
                    SetupTurnQueue();
                });
        }

        /// <summary>
        /// Sets up the initial timeline at the start of the game.
        /// </summary>
        private void SetupTurnQueue()
        {
            turnManager.SetupTurnQueue();
        }
    }
}
