using Managers;
using TMPro;
using Turn;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    [RequireComponent(typeof(Button))]
    public class TimelineInsightButton : DialogueComponent<GameDialogue>
    {
        [SerializeField] private Button btn;
        [SerializeField] private TextMeshProUGUI text;
        private TurnManager turnManager;
        private GameDialogue.UnitInfo selectedUnit = null;
        
        #region Drawing

        internal void Destroy()
        {
            DestroyImmediate(gameObject);
        }

        #endregion


        #region UIComponent

        protected override void OnComponentAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.meditateConfirmed.AddListener(MeditateConfirmed);
            dialogue.unitSelected.AddListener(unitSelected);
            dialogue.unitDeselected.AddListener(unitDeselected);
            dialogue.turnManipulated.AddListener(UpdateText);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.meditateConfirmed.RemoveListener(MeditateConfirmed);
            dialogue.unitSelected.RemoveListener(unitSelected);
            dialogue.unitDeselected.RemoveListener(unitDeselected);
            dialogue.turnManipulated.RemoveListener(UpdateText);
        }

        #endregion


        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            
            EnableBtn(info);
            UpdateText(new GameDialogue.UnitInfo());
        }

        private void MeditateConfirmed(GameDialogue.UnitInfo unitInfo)
        {
            UpdateText(new GameDialogue.UnitInfo());
            EnableBtn();
        }

        private void unitSelected(GameDialogue.UnitInfo unitInfo)
        {
            selectedUnit = unitInfo;
            EnableBtn();
            }
        private void unitDeselected()
        {
            selectedUnit = null;
            EnableBtn();
            }

        #endregion

        #region FunctionalityS

        public void OnClick()
        {
            if (turnManager.turnManipulating)
            {
                turnManager.turnManipulating = false;
                dialogue.turnManipulationEnded.Invoke();
            }
            else if (selectedUnit != null)
            {
                dialogue.turnManipulationStarted.Invoke(selectedUnit);
                turnManager.turnManipulating = true;
            }

        }

        private void EnableBtn(GameDialogue.TurnInfo info)
        {
            btn.interactable = (info.IsPlayer && selectedUnit != null && (turnManager.Insight.Value >= 2));
        }
        private void EnableBtn()
        {
            //btn.interactable = (turnManager.ActingPlayerUnit != null && selectedUnit != null && (turnManager.Insight.Value >= 2));
            btn.interactable = (turnManager.ActingPlayerUnit != null && selectedUnit != null);
        }

        private void UpdateText(GameDialogue.UnitInfo unitInfo) => text.text = turnManager.Insight.Value.ToString();

        #endregion
    }
}
