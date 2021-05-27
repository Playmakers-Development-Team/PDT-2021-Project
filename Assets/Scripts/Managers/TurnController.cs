using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private List<GameObject> allUnitCards;

        private GameObject currentTurnIndicator;
        private UnitManager unitManager;
        private TurnManager turnManager;

     
        
        //TODO Have a way to not cheat the start method race condition
        private IEnumerator Start()
        {
            turnManager = ManagerLocator.Get<TurnManager>();

            yield return new WaitForSeconds(0.1f);
            turnManager.SetupTurnQueue();
            IReadOnlyList<IUnit> allUnits = turnManager.CurrentTurnQueue;
            foreach (IUnit unit in allUnits)
            {
                var unitCard = Instantiate(unitCardPrefab, timeline);
                unitCard.GetComponent<UnitCard>().SetCardImageAs(unit);
                unitCard.GetComponent<UnitCard>().SetUnit(unit);
                unitCard.GetComponent<UnitCard>().SetUnitText(unit.ToString());
                allUnitCards.Add(unitCard);
            }
            
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (GameObject unitcard in allUnitCards) 
            {
                if (unitcard.GetComponent<UnitCard>().Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitcard.transform);
                }
            }
            
            turnManager.onTurnEnd += UpdateTurnUI;
            turnManager.onRoundStart += UpdateForNewRound;

        }

        private void UpdateTurnUI(TurnManager turnManager)
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (GameObject unitcard in allUnitCards)
            {
                if (unitcard.GetComponent<UnitCard>().Unit == turnManager.PreviousUnit)
                    unitcard.GetComponent<Image>().color = Color.black;

                if (unitcard.GetComponent<UnitCard>().Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitcard.transform);
                    unitcard.GetComponent<Image>().color = Color.red;

                }
                
            }
        }

        private void UpdateForNewRound(TurnManager turnManager)
        {
            if (currentTurnIndicator != null)
                Destroy(currentTurnIndicator);
            
            foreach (GameObject unitcard in allUnitCards) 
            {
                unitcard.GetComponent<Image>().color = Color.red;

                if (unitcard.GetComponent<UnitCard>().Unit == turnManager.CurrentUnit)
                {
                    currentTurnIndicator = Instantiate(currentTurnIndicatorPrefab, unitcard.transform);
                }
            }
            
            

        }
        
    }
}
