using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimelineUI : MonoBehaviour
    {
        [SerializeField] private Transform timeline;
        [SerializeField] private GameObject unitCardPrefab;

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
                Debug.Log($"CurrentSpeed {unit.Speed}");
                var unitCard = Instantiate(unitCardPrefab, timeline);
                unitCard.GetComponent<UnitCard>().SetCardImageAs(unit);
                unitCard.GetComponent<UnitCard>().SetUnitText(unit.ToString());

            }

        }

    }

}
