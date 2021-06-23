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
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (IUnit unit in turnManager.CurrentTurnQueue)
            {
                CreateUnitCard(unit);
            }

            turnManager.onTurnEnd += UpdateTurnUI;
            turnManager.onRoundStart += UpdateForNewRound;
            turnManager.onUnitDeath += RefreshTimelineUI;
            turnManager.newUnitAdded += AddUnitToTimeline;
        }
        
        /// <summary>
        /// Updates the timeline UI when a new turn has started, leaving the previous unit greyed out.
        /// </summary>
        /// <param name="turnManager"></param>
        private void UpdateTurnUI(TurnManager turnManager)
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);

            foreach (UnitCard unitCard in allUnitCards)
            {
                if (turnManager.PreviousUnit != null)
                {
                    if (unitCard.Unit == turnManager.PreviousUnit)
                        unitCard.GetComponent<Image>().color = Color.black;
                }

                if (unitCard.Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                    unitCard.GetComponent<Image>().color = Color.red;
                }
            }
        }
        
        /// <summary>
        /// Completely refreshes the timeline UI.
        /// </summary>
        /// <param name="turnManager"></param>
        private void RefreshTimelineUI(TurnManager turnManager)
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
                if (unitCard.Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                }
            }
        }
    
        /// <summary>
        /// Updates the timeline with the new unit.
        /// </summary>
        /// <param name="turnManager"></param>
        private void AddUnitToTimeline(TurnManager turnManager)
        {
            var allUnits = ManagerLocator.Get<UnitManager>().GetAllUnits();

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
            
            if (unit == turnManager.CurrentUnit)
            {
                currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
            }
        }
        
        /// <summary>
        /// Updates the timeline UI for the new round.
        /// </summary>
        /// <param name="turnManager"></param>
        private void UpdateForNewRound(TurnManager turnManager)
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
