using Managers;
using TMPro;
using Turn;
using UI.Core;
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
        private GameDialogue.UnitInfo selectedUnit;

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
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.meditateConfirmed.RemoveListener(MeditateConfirmed);
            dialogue.unitSelected.RemoveListener(unitSelected);
            dialogue.unitDeselected.RemoveListener(unitDeselected);
        }

        #endregion


        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            EnableBtn(info);
            UpdateText();
        }

        private void MeditateConfirmed(GameDialogue.UnitInfo unitInfo)
        {
            UpdateText();
        }

        private void unitSelected(GameDialogue.UnitInfo unitInfo)
        {
            btn.interactable = true;
            selectedUnit = unitInfo;
        }
        private void unitDeselected()
        {
            btn.interactable = false;
            selectedUnit = null;
        }

        #endregion

        #region FunctionalityS

        public void OnClick()
        {
            if (selectedUnit != null)
                dialogue.turnManipulationStarted.Invoke(selectedUnit);

        }

        private void EnableBtn(GameDialogue.TurnInfo info)
        {
            btn.interactable = info.IsPlayer;
        }

        private void UpdateText() => text.text = turnManager.Insight.Value.ToString();

        #endregion
    }
}
