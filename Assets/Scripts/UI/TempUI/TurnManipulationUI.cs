using System.Collections.Generic;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UnityEngine;

namespace UI.TempUI
{
    // TODO: Delete once proper turn manipulation UI has been added
    public class TurnManipulationUI : MonoBehaviour
    {
        public GameObject turnManipulateButton;
        [SerializeField] private Transform parent;

        private TurnManager turnManager;
        private CommandManager commandManager;

        private List<GameObject> allButtons = new List<GameObject>();

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable() =>
            commandManager.ListenCommand<TurnManipulatedCommand>(OnTurnManipulated);

        private void OnDisable() =>
            commandManager.UnlistenCommand<TurnManipulatedCommand>(OnTurnManipulated);

        private void OnTurnManipulated(TurnManipulatedCommand cmd) => DestroyButtons();

        private void DestroyButtons()
        {
            foreach (var button in allButtons)
                Destroy(button);
        }

        // BUG: Not working
        public void ManipulateBefore()
        {
            if (!turnManager.UnitCanDoTurnManipulation(turnManager.ActingUnit))
                return;
            
            for (int i = 0; i < turnManager.CurrentTurnQueue.Count; i++)
            {
                if (i > turnManager.CurrentTurnIndex && turnManager.UnitCanBeTurnManipulated
                    (turnManager.CurrentTurnQueue[i]))
                {
                    GameObject temp = Instantiate(turnManipulateButton, parent);
                    temp.GetComponent<TurnManipulateTargetUI>().
                        InitialiseButton(turnManager.CurrentTurnQueue[i], parent, true);
                    allButtons.Add(temp);
                }
            }
        }

        // TODO: Update timeline UI
        public void ManipulateAfter()
        {
            if (!turnManager.UnitCanDoTurnManipulation(turnManager.ActingUnit))
                return;
            
            for (int i = 0; i < turnManager.CurrentTurnQueue.Count; i++)
            {
                if (i > turnManager.CurrentTurnIndex && turnManager.UnitCanBeTurnManipulated
                (turnManager.CurrentTurnQueue[i]))
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
