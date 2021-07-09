using Units.Commands;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField] private GameObject[] preMadeTimeline;
        [SerializeField] private bool isTimelineRandomised;
        
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
            if (isTimelineRandomised)
                turnManager.SetupTurnQueue();
            else 
                turnManager.SetupTurnQueue(preMadeTimeline);
        }
    }
}
