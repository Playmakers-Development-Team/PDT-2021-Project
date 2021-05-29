using System.Collections;
using System.Collections.Generic;
using Commands;
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
        }

        private void UpdateTurnUI(TurnManager turnManager)
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (UnitCard unitCard in allUnitCards)
            {
                if (unitCard.Unit == turnManager.PreviousUnit)
                    unitCard.GetComponent<Image>().color = Color.black;

                if (unitCard.Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                    unitCard.GetComponent<Image>().color = Color.red;
                }
            }
        }

        private void UpdateForNewRound(TurnManager turnManager)
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (UnitCard unitCard in allUnitCards) 
            {
                unitCard.GetComponent<Image>().color = Color.red;

                if (unitCard.Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitCard.transform);
                }
            }
        }
    }
}
