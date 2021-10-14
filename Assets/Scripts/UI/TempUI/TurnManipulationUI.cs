using System.Collections.Generic;
using Commands;
using Managers;
using TMPro;
using Turn;
using Turn.Commands;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TempUI
{
    // TODO: Delete once proper turn manipulation UI has been added
    public class TurnManipulationUI : MonoBehaviour
    {
        public GameObject turnManipulateButton;
        [SerializeField] private TextMeshProUGUI manipulateBeforeText;
        [SerializeField] private TextMeshProUGUI manipulateAfterText;
        [SerializeField] private Transform parent;

        private TurnManager turnManager;
        private CommandManager commandManager;
        private bool canPressManipulate = true;
        private List<GameObject> allButtons = new List<GameObject>();

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand<StartTurnCommand>(OnStartTurn);
            commandManager.ListenCommand<EndTurnCommand>(OnEndTurn);
            commandManager.ListenCommand<TurnManipulatedCommand>(OnTurnManipulated);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand<StartTurnCommand>(OnStartTurn);
            commandManager.UnlistenCommand<EndTurnCommand>(OnEndTurn);
            commandManager.UnlistenCommand<TurnManipulatedCommand>(OnTurnManipulated);
        }
        
        private void OnStartTurn(StartTurnCommand cmd)
        {
            if (!(cmd.Unit is PlayerUnit))
                return;
            
            if (manipulateBeforeText != null)
                manipulateBeforeText.text = $"Move a unit's turn to before {cmd.Unit.Name}";
            
            if (manipulateAfterText != null)
                manipulateAfterText.text = $"Move a unit's turn to after {cmd.Unit.Name}";
        }
        
        private void OnEndTurn(EndTurnCommand cmd) => DestroyButtons();

        private void OnTurnManipulated(TurnManipulatedCommand cmd) => DestroyButtons();

        private void DestroyButtons()
        {
            canPressManipulate = true;
            
            foreach (var button in allButtons)
                Destroy(button);
        }

        public void ManipulateBefore()
        {
            if (!turnManager.UnitCanDoTurnManipulation(turnManager.ActingUnit))
                return;
            
            if (!canPressManipulate)
            {
                Debug.LogWarning("You are already manipulating turns!");
                return;
            }

            canPressManipulate = false;
            
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
            
            if (!canPressManipulate)
            {
                Debug.LogWarning("You are already manipulating turns!");
                return;
            }

            canPressManipulate = false;
            
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
