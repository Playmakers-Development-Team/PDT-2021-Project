using System;
using System.Collections.Generic;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Players;
using UnityEngine;

namespace UI
{
    //TODO DELETE THIS CLASS 
    public class TurnManipulationUI : MonoBehaviour
    {
        public GameObject turnManipulateButton;
        [SerializeField] private Transform parent;
        [SerializeField] private TurnController turnController;

        private PlayerManager playerManager;

        private TurnManager turnManager;

        private List<GameObject> allButtons = new List<GameObject>();

        private List<IUnit> currentUnits = new List<IUnit>();

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        private void Start()
        {
            turnController = GameObject.FindGameObjectWithTag("TurnController").
                GetComponent<TurnController>();

            ManagerLocator.Get<CommandManager>().
                ListenCommand<TurnManipulatedCommand>(cmd =>
                {
                    DestroyButtons();
                });
        }

        private void DestroyButtons()
        {
            foreach (var button in allButtons)
                Destroy(button);
        }

        public void ManipulateBefore()
        {
            if (!turnManager.UnitCanDoTurnManipulation(turnManager.ActingUnit) ||
                playerManager.Insight.Value <= 0)
                return;

            Debug.Log($"CURRENT INDEX {turnManager.PhaseIndex}");

            for (int i = 0; i < turnManager.CurrentTurnQueue.Count; i++)
            {
                if (i > turnManager.CurrentTurnIndex)
                {
                    GameObject temp = Instantiate(turnManipulateButton, parent);
                    temp.GetComponent<TurnManipulateTargetUI>().
                        InitialiseButton(turnManager.CurrentTurnQueue[i], parent, true);
                    allButtons.Add(temp);
                }
            }
        }

        public void ManipulateAfter()
        {
            if (!turnManager.UnitCanDoTurnManipulation(turnManager.ActingUnit) ||
                playerManager.Insight.Value <= 0)
                return;

            for (int i = 0; i < turnManager.CurrentTurnQueue.Count; i++)
            {
                if (i > turnManager.CurrentTurnIndex)
                {
                    GameObject temp = Instantiate(turnManipulateButton, parent);
                    temp.GetComponent<TurnManipulateTargetUI>().
                        InitialiseButton(turnManager.CurrentTurnQueue[i], parent, false);
                    allButtons.Add(temp);
                }
            }
        }
    }
}
