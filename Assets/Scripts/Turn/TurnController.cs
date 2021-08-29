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

            if (!isTimelineRandomised)
            {
                foreach (GameObject gameObject in preMadeTimeline)
                {

                    if (gameObject.GetComponent<IUnit>() is null)
                    {
                        Debug.LogError(
                            $"WARNING: {gameObject.name} is not an IUnit the game will not run! Please remove the unit or make it an IUnit");
                        UnityEditor.EditorApplication.isPlaying = false;
                    }
                }
            }

            if (isTimelineRandomised)
                turnManager.SetupTurnQueue(turnPhases);
            else 
                turnManager.SetupTurnQueue(preMadeTimeline,turnPhases);
        }

        // TODO: Can be removed once proper UI is in place.
         public void Meditate() => turnManager.Meditate();
        
    }
}
