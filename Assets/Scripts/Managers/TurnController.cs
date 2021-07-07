using System.Collections.Generic;
using System.Linq;
using Commands;
using UI;
using Units;
using Units.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        /// <summary>
        /// The Transform for the timeline, used as the parent to instantiate the unit cards.
        /// </summary>
        [SerializeField] private Transform timeline;
        
        /// <summary>
        /// The prefab for the unit card.
        /// </summary>
        [SerializeField] private GameObject unitCardPrefab;
        
        /// <summary>
        /// The prefab for the current turn indicator.
        /// </summary>
        [SerializeField] private GameObject currentTurnIndicatorPrefab;
        
        /// <summary>
        /// A list of all the unit cards shown in the timeline.
        /// </summary>
        [SerializeField] private List<UnitCard> allUnitCards;
        
        [SerializeField]
          
        /// <summary>
        /// The GameObject for the current turn indicator.
        /// </summary>
        private GameObject currentTurnIndicator;
        
        /// <summary>
        /// A reference to the TurnManager.
        /// </summary>
        private TurnManager turnManager;

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.ListenCommand<StartTurnCommand>(cmd => UpdateTurnUI());
            commandManager.ListenCommand<StartRoundCommand>(cmd => UpdateForNewRound());

            commandManager.CatchCommand<PlayerUnitsReadyCommand, EnemyUnitsReadyCommand>(
                (cmd1, cmd2) =>
                {
                    SetupTurnQueue();
                    commandManager.ListenCommand<KilledUnitCommand>(cmd => RefreshTimelineUI());
                });
        }

        /// <summary>
        /// Sets up the initial timeline at the start of the game.
        /// </summary>
        private void SetupTurnQueue()
        {
            turnManager.SetupTurnQueue();
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (IUnit unit in turnManager.CurrentTurnQueue)
            {
                CreateUnitCard(unit);
            }
        }
        
        /// <summary>
        /// Updates the timeline UI when a new turn has started, leaving the previous unit greyed out.
        /// </summary>
        private void UpdateTurnUI()
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);

            foreach (UnitCard unitCard in allUnitCards)
            {
                if (turnManager.PreviousActingUnit != null)
                {
                    if (turnManager.FindTurnIndexFromCurrentQueue(unitCard.Unit) < turnManager.CurrentTurnIndex)
                        unitCard.GetComponent<Image>().color = Color.black;
                }
                
                if (unitCard.Unit == turnManager.ActingUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                    unitCard.GetComponent<Image>().color = Color.red;
                }
            }
        }
        
        /// <summary>
        /// Completely refreshes the timeline UI.
        /// </summary>
        private void RefreshTimelineUI()
        {
            foreach (UnitCard unitCard in allUnitCards)
            {
                if (unitCard.Unit == turnManager.RecentUnitDeath)
                {
                    Destroy(unitCard.gameObject);
                    allUnitCards.Remove(unitCard);
                    break;
                }
            }
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (UnitCard unitCard in allUnitCards)
            {
                if (unitCard.Unit == turnManager.ActingUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                }
            }
        }
    
        /// <summary>
        /// Updates the timeline with the new unit.
        /// </summary>
        private void AddUnitToTimeline()
        {
            var allUnits = ManagerLocator.Get<UnitManager>().AllUnits;

            foreach (var unit in allUnits)
            {
                if (allUnitCards.All(unitCard => unitCard.Unit != unit))
                {
                    CreateUnitCard(unit);
                    
                    break;
                }
            }
        }

        private void CreateUnitCard(IUnit unit)
        {
            var unitCardObject = Instantiate(unitCardPrefab, timeline);
            var unitCard = unitCardObject.GetComponent<UnitCard>();
            unitCard.SetUnit(unit);

            allUnitCards.Add(unitCard);
            
            if (unit == turnManager.ActingUnit)
            {
                currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
            }
        }
        
        /// <summary>
        /// Updates the timeline UI for the new round.
        /// </summary>
        private void UpdateForNewRound()
        {
            allUnitCards.ForEach(unitCard =>
            {
                Destroy(unitCard.gameObject);
            });
            
            allUnitCards.Clear();
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (IUnit unit in turnManager.CurrentTurnQueue)
            {
                CreateUnitCard(unit);
            }
        }
    }
}
