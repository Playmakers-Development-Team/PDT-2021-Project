using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        
        //TODO DELETE THIS BEFORE MERGE
        [Tooltip("Debug Tool to check if you can turn manipulate")] 
        [SerializeField] private bool maniplulateTurn;
        
        /// <summary>
        [SerializeField] private TurnManager.TurnPhases[] turnPhases;
        [Tooltip("The global turn phase for every player unit")] 
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


        //TODO DELETE THIS FUNCTION BEFORE MERGE
        private void OnValidate()
        {
            if (maniplulateTurn)
                turnManager.ShiftTurnQueue(1,3);
        }

        /// <summary>
        /// Sets up the initial timeline at the start of the game.
        /// </summary>
        private void SetupTurnQueue()
        {
            turnManager.SetupTurnQueue(turnPhases);
        }
    }
}
