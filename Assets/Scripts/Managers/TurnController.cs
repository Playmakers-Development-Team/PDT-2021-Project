using System.Collections;
using System.Collections.Generic;
using System.Text;
using Commands;
using Cysharp.Threading.Tasks;
using UI;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField] private Transform timeline;
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private GameObject currentTurnIndicatorPrefab;
        [SerializeField] private List<UnitCard> allUnitCards;

        [SerializeField] private List<IUnit> lastKnownUnits;

        private GameObject currentTurnIndicator;
        private TurnManager turnManager;
        private bool isPlayerUnitsReady;
        private bool isEnemyUnitsReady;
        
        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();
            
            commandManager.ListenExecuteCommand<EnemyUnitsReadyCommand>(cmd =>
            {
                isEnemyUnitsReady = true;
                SetupTurnQueue();
            });
            commandManager.ListenExecuteCommand<PlayerUnitsReadyCommand>(cmd =>
            {
                isPlayerUnitsReady = true;
                SetupTurnQueue();
            });
        }

        private void SetupTurnQueue()
        {
            if (!isPlayerUnitsReady || !isEnemyUnitsReady)
                return;
            
            turnManager.SetupTurnQueue();
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (IUnit unit in turnManager.CurrentTurnQueue)
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

            turnManager.onTurnEnd += UpdateTurnUI;
            turnManager.onRoundStart += UpdateForNewRound;
            turnManager.onUnitDeath += RefreshTimelineUI;
            turnManager.newUnitAdded += AddUnitToTimeline;
        }
        
        /// <summary>
        /// Updates the timeline UI when a new turn has started, leaving the previous unit  greyed out
        /// </summary>
        /// <param name="turnManager"></param>
        private void UpdateTurnUI(TurnManager turnManager)
        {
            
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);

            foreach (UnitCard unitCard in allUnitCards)
            {
                if (turnManager.PreviousUnit != null){
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
        /// Completely refreshes the timeline UI
        /// </summary>
        /// <param name="turnManager"></param>
        private void RefreshTimelineUI(TurnManager turnManager)
        {
            allUnitCards.ForEach(unitCard =>
            {
                if (unitCard.Unit != turnManager.RecentUnitDeath)
                    return;
                
                Destroy(unitCard.gameObject);
            });
            
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

        private void AddUnitToTimeline(TurnManager turnManager)
        {
            var allUnits = ManagerLocator.Get<UnitManager>().GetAllUnits();

            var flag = false;

            foreach (var unit in allUnits)
            {
                foreach (var unit2 in allUnitCards)
                {
                    if (unit == unit2.Unit)
                    {
                        flag = true;
                        break;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                Debug.Log("current flag " + flag);
                if (!flag)
                {
                    var unitCardObject = Instantiate(unitCardPrefab, timeline);
                    var unitCard = unitCardObject.GetComponent<UnitCard>();
                    unitCard.SetUnit(unit);

                    allUnitCards.Add(unitCard);
                    break;
                }
            }
            
            // for (var i = 0; i < allUnits.Count; i++)
            // {
            //     foreach (var t in allUnitCards)
            //     {
            //         if (allUnits[i] != t.Unit)
            //             continue;
            //         
            //         flag = true;
            //         break;
            //     }
            //
            //     if (!flag)
            //         continue;
            //     
            //     var unitCardObject = Instantiate(unitCardPrefab, timeline);
            //     var unitCard = unitCardObject.GetComponent<UnitCard>();
            //     unitCard.SetUnit(allUnits[i]);
            //     allUnitCards.Add(unitCard);
            //     break;
            //
            // }
           
            
                
        }
        
        
        /// <summary>
        /// Updates the timeline UI for the new round
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
                var unitCardObject = Instantiate(unitCardPrefab, timeline);
                var unitCard = unitCardObject.GetComponent<UnitCard>();

                unitCard.SetUnit(unit);

                allUnitCards.Add(unitCard);

                if (unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                }
            }

            // if (currentTurnIndicator != null)
            //     Destroy(currentTurnIndicator);
            //
            // foreach (UnitCard unitCard in allUnitCards) 
            // {
            //     unitCard.GetComponent<Image>().color = Color.red;
            //
            //     if (unitCard.Unit == turnManager.CurrentUnit)
            //     {
            //         currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
            //     }
            // }
        }
    }
}
