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
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.meditateConfirmed.RemoveListener(MeditateConfirmed);
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

        #endregion

        #region FunctionalityS

        public void OnClick()
        {
            Debug.Log("insightClicked");
        }

        private void EnableBtn(GameDialogue.TurnInfo info)
        {
            btn.interactable = info.IsPlayer;
        }

        private void UpdateText() => text.text = turnManager.Insight.Value.ToString();

        #endregion
    }
}
