using Commands;
using Grid.GridObjects;
using Managers;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;

namespace Turn
{
    public class TurnController : MonoBehaviour
    {
        [Tooltip("The global phase each player unit has to follow sequentially")]
        [SerializeField] private TurnManager.TurnPhases[] turnPhases;
        
        [Tooltip("The global turn phase for every player unit")]
        [SerializeField] private GameObject[] preMadeTimeline;
        
        [SerializeField] private bool isTimelineRandomised;
        [Tooltip("If ability speed is enabled, the pre made timeline will only be used for the" +
                 "first round. Disabling ability speed can be useful for testing purposes.")]
        [SerializeField] private bool disableAbilitySpeed;
       
        private TurnManager turnManager;
        private UnitManager unitManager;
        
        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.CatchCommand<UnitsReadyCommand<PlayerUnitData>, UnitsReadyCommand<EnemyUnitData>>(
                (cmd1, cmd2) =>
                {
                    SetupTurnQueue();
                });
            
            turnManager.Reset();
        }

        /// <summary>
        /// Sets up the initial timeline at the start of the game.
        /// </summary>
        private void SetupTurnQueue()
        {
            turnManager.AbilitySpeedEnabled = !disableAbilitySpeed;
            
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

        // TODO: Can be removed once proper UI is in place
        public void Meditate()
        {
            turnManager.Meditate();
        }
    }
}
