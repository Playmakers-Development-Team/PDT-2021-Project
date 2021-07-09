using Units.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        
        //TODO DELETE THIS BEFORE MERGE
        [Tooltip("Debug Tool to check if you can turn manipulate")] 
        [SerializeField] private bool maniplulateTurn;
        
        [Tooltip("The global turn phase for every player unit")]
        [SerializeField] private TurnManager.TurnPhases[] turnPhases;
        
        
        private TurnManager turnManager;

        private UnitManager unitManager;
        

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.CatchCommand<PlayerUnitsReadyCommand, EnemyUnitsReadyCommand>(
                (cmd1, cmd2) =>
                {
                    SetupTurnQueue();
                });
        }

        public void Meditate()
        {
            Debug.Log("meditated c:");
            if (turnManager.Meditate())
            {
                unitManager.IncrementInsight(1);
            }
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
