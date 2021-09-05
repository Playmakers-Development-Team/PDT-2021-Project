using System.Collections.Generic;
using Managers;
using Turn;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    public class TimelinePanel : DialogueComponent<GameDialogue>
    {
        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private GameObject portraitPrefab;
        [SerializeField] private GameObject dividerPrefab;
        [SerializeField] private GameObject insightButtonPrefab;
        [SerializeField] private int timelineLength = 8;

        private TurnManager turnManager;

        private readonly List<TimelinePortrait> portraits = new List<TimelinePortrait>();


        #region UIComponent

        protected override void OnComponentAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.turnManipulated.AddListener(OnTurnManipulated);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.turnManipulated.RemoveListener(OnTurnManipulated);
        }

        #endregion


        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            UpdatePortraits();
        }

        private void OnTurnManipulated(GameDialogue.TurnInfo info) => UpdatePortraits();

        public void OnDelayButtonPressed()
        {
            if (turnManager.ActingPlayerUnit == null)
                return;

            dialogue.meditateConfirmed.Invoke(dialogue.GetInfo(turnManager.ActingPlayerUnit));
        }

        #endregion


        #region Portrait Management

        private void UpdatePortraits()
        {
            ClearPortraits();

            List<IUnit> currentTurnQueue = new List<IUnit>(turnManager.CurrentTurnQueue);
            int startIndex = turnManager.CurrentTurnIndex;
            int roundcount = turnManager.RoundCount;
            //currentTurnQueue.RemoveRange(0, startIndex);

            CreateInsightButton();
            CreatePortraits(currentTurnQueue, startIndex);
            Debug.Log("roundcount");

            // CreatePortraits(currentTurnQueue);
            // CreateDivider();
            // CreatePortraits(turnManager.NextTurnQueue);
        }

        private void ClearPortraits()
        {
            for (int i = portraits.Count - 1; i >= 0; i--)
                portraits[i].Destroy();

            portraits.Clear();
        }

        private void CreatePortraits(IEnumerable<IUnit> units)
        {
            foreach (IUnit unit in units)
            {
                GameObject obj = Instantiate(portraitPrefab, scrollRect.content);
                TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

                GameDialogue.UnitInfo info = dialogue.GetInfo(unit);
                portrait.Assign(info);
                portraits.Add(portrait);
            }
        }

        private void CreatePortraits(List<IUnit> units, int startIndex)
        {
            int index = startIndex;
            int count = 0;

            while (count < timelineLength)
            {
                if (index > units.Count - 1)
                {
                    CreateDivider();
                    count++;
                    if (count >= timelineLength)
                        break;

                    while (index > units.Count - 1)
                        index = index - units.Count;
                }

                CreatePortrait(units[index]);

                index++;
                count++;
            }
        }

        private void CreatePortrait(IUnit unit)
        {
            GameObject obj = Instantiate(portraitPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            GameDialogue.UnitInfo info = dialogue.GetInfo(unit);
            portrait.Assign(info);
            portraits.Add(portrait);
        }

        private void CreateDivider()
        {
            GameObject obj = Instantiate(dividerPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            portraits.Add(portrait);
        }

        private void CreateInsightButton()
        {
            GameObject obj = Instantiate(insightButtonPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            portraits.Add(portrait);
        }

        #endregion
    }
}
