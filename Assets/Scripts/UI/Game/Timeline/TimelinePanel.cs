using System.Collections.Generic;
using Managers;
using Turn;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimelinePanel : UIComponent<GameDialogue>
    {
        [SerializeField] private ScrollRect scrollRect;
        
        [SerializeField] private GameObject portraitPrefab;
        [SerializeField] private GameObject dividerPrefab;
        
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
        }
        
        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }
        
        #endregion
        

        #region Listeners
        
        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            UpdatePortraits();
        }

        public void OnDelayButtonPressed()
        {
            if (turnManager.ActingPlayerUnit == null)
                return;
            
            // TODO: If anything else needs to know when this button is pressed, it'll need to be moved to an Event...
            dialogue.delayConfirmed.Invoke(dialogue.GetInfo(turnManager.ActingPlayerUnit));
        }

        #endregion
        
        
        #region Portrait Management
        
        private void UpdatePortraits()
        {
            ClearPortraits();

            List<IUnit> currentTurnQueue = new List<IUnit>(turnManager.CurrentTurnQueue);
            int startIndex = turnManager.CurrentTurnIndex;
            currentTurnQueue.RemoveRange(0, startIndex);
            
            CreatePortraits(currentTurnQueue);
            CreateDivider();
            CreatePortraits(turnManager.NextTurnQueue);
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
                
                portrait.Assign(unit);
                portraits.Add(portrait);
            }
        }

        private void CreateDivider()
        {
            GameObject obj = Instantiate(dividerPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            portraits.Add(portrait);
        }
        
        #endregion
    }
}
