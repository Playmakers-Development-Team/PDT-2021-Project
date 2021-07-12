using Units.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
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

        public void Meditate()
        {
            Debug.Log("meditated c:");
            if (turnManager.Meditate())
            {
                unitManager.IncrementInsight(1);
            }
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
