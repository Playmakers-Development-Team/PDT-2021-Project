using Units.Commands;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        
        //TODO DELETE THIS BEFORE MERGE
        [Tooltip("Debug Tool to check if you can turn manipulate")] 
        [SerializeField] private bool maniplulateTurn;
        
        [Tooltip("The global phase each player unit has to follow sequentially")]
        [SerializeField] private TurnManager.TurnPhases[] turnPhases;
        
        [Tooltip("The global turn phase for every player unit")]
        [SerializeField] private GameObject[] preMadeTimeline;
        
        [SerializeField] private bool isTimelineRandomised;
       
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
            if (preMadeTimeline.Length < unitManager.AllUnits.Count)
            {
                isTimelineRandomised = true;
                Debug.Log("Timeline was not filled or completed, automatically setting it to randomised");
            }
        
            if (isTimelineRandomised)
                turnManager.SetupTurnQueue(turnPhases);
            else 
                turnManager.SetupTurnQueue(preMadeTimeline,turnPhases);
        }
    }
}
