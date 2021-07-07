using System;
using Managers;
using UnityEngine;

namespace UI
{
    //TODO DELETE THIS CLASS 
    public class TurnManipulationUI : MonoBehaviour
    {

         public GameObject turnManipulateButton;
        [SerializeField] private TurnController turnController;
        private TurnManager turnManager;
        

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        private void Start()
        {
            turnController = GameObject.FindGameObjectWithTag("TurnController").
                GetComponent<TurnController>();
        }

        public void ManipulateBefore()
        {
            foreach (var unit in turnManager.CurrentTurnQueue)
            {
                if (unit != turnManager.ActingUnit)
                {
                    foreach(UnitCard unitcard in turnController.allUnitCards)
                    {
                        if (unit == unitcard.Unit)
                        {
                            GameObject btnUnit = Instantiate(turnManipulateButton,unitcard.transform);
                            btnUnit.GetComponent<TurnManipulateTargetUI>().InitialiseButton(unit,
                            unitcard.transform,true);
                        }
                    }
                }
            }
        }
        
        public void ManipulateAfter()
        {
            foreach (var unit in turnManager.CurrentTurnQueue)
            {
                if (unit != turnManager.ActingUnit)
                {
                    foreach(UnitCard unitcard in turnController.allUnitCards)
                    {
                        if (unit == unitcard.Unit)
                        {
                            GameObject btnUnit = Instantiate(turnManipulateButton,unitcard.transform);
                            btnUnit.GetComponent<TurnManipulateTargetUI>().InitialiseButton(unit,
                                unitcard.transform,false);
                        }
                    }
                }
            }
        }
        
        
        
    }
}
