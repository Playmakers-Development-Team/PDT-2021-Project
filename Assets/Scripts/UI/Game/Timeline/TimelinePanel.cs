using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
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
        [SerializeField] private int timelineLength = 7;
        [SerializeField] private bool drawInsightBtn = false;
        
        [Header("Transition")]
        
        [SerializeField] private Animator animator;
        [SerializeField] private float delay;

        private TurnManager turnManager;

        private readonly List<GameObject> portraits = new List<GameObject>();
        private static readonly int promoted = Animator.StringToHash("promoted");
        private static readonly int demoted = Animator.StringToHash("demoted");


        #region UIComponent

        protected override void OnComponentAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.turnManipulated.AddListener(OnTurnManipulated);
            
            dialogue.promoted.AddListener(OnPromoted);
            dialogue.demoted.AddListener(OnDemoted);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.turnManipulated.RemoveListener(OnTurnManipulated);
            
            dialogue.promoted.RemoveListener(OnPromoted);
            dialogue.demoted.RemoveListener(OnDemoted);
        }

        protected override void OnComponentStart()
        {
            if (manager.Peek() == dialogue)
                TransitionIn();
        }

        #endregion


        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            UpdatePortraits();
        }

        private void OnTurnManipulated(GameDialogue.UnitInfo unitInfo)
        {
            UpdatePortraits();
            dialogue.unitDeselected.Invoke();
            dialogue.turnManipulationEnded.Invoke();
        }

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
            
            
                CreateInsightButton();
                currentTurnQueue.RemoveRange(0, startIndex);
                List<IUnit> appendTurnQueue = new List<IUnit>(turnManager.CurrentTurnQueue);
                appendTurnQueue.RemoveRange(startIndex, appendTurnQueue.Count - startIndex);
                currentTurnQueue.AddRange(appendTurnQueue);
                CreatePortraits(currentTurnQueue);

        }

        private void ClearPortraits()
        {
            for (int i = portraits.Count - 1; i >= 0; i--)
                Destroy(portraits[i]);

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
                portraits.Add(obj);
            }
        }

        //used for old multi turn
        private void CreatePortraits(List<IUnit> units, int startIndex)
        {
            int count = 0;
            int roundcount = turnManager.RoundCount + 2;

            while (count < timelineLength)
            {
                if (startIndex > units.Count - 1)
                {
                    CreateDivider(roundcount++);
                    count++;
                    if (count >= timelineLength)
                        break;

                    while (startIndex > units.Count - 1)
                        startIndex = startIndex - units.Count;
                }

                CreatePortrait(units[startIndex]);

                startIndex++;
                count++;
            }
        }

        private void CreatePortrait(IUnit unit)
        {
            GameObject obj = Instantiate(portraitPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            GameDialogue.UnitInfo info = dialogue.GetInfo(unit);
            portrait.Assign(info);
            portraits.Add(obj);
        }

        private void CreateDivider(int round)
        {
            GameObject obj = Instantiate(dividerPrefab, scrollRect.content);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = round.ToString();
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            portraits.Add(obj);
        }

        private void CreateInsightButton()
        {
            GameObject obj = Instantiate(insightButtonPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();
            obj.GetComponentInChildren<TextMeshProUGUI>().text =
                turnManager.Insight.Value.ToString();

            portraits.Add(obj);
        }

        #endregion
        
        #region Transition
        
        private async void TransitionIn()
        {
            await UniTask.Delay((int) (delay * 1000.0f));
            OnPromoted();
        }

        private void OnPromoted() => animator.SetTrigger(promoted);

        private void OnDemoted() => animator.SetTrigger(demoted);
        
        #endregion
    }
}
