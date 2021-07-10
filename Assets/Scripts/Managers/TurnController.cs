using Units.Commands;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
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
                turnManager.SetupTurnQueue();
            else 
                turnManager.SetupTurnQueue(preMadeTimeline);
        }
    }
}
