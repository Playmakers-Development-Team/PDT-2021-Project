using System.Collections.Generic;
using Managers;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Refactored
{
    public class TimelinePanel : Element
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button delayButton;
        
        [SerializeField] private GameObject portraitPrefab;
        [SerializeField] private GameObject dividerPrefab;
        
        private readonly List<TimelinePortrait> portraits = new List<TimelinePortrait>();
        private TurnManager turnManager;

        protected override void OnAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            
            manager.turnChanged.AddListener(OnTurnChanged);
        }

        private void OnTurnChanged()
        {
            UpdatePortraits();
        }

        public void OnDelayButtonPressed()
        {
            Debug.Log("Delay button pressed...");
        }

        private void UpdatePortraits()
        {
            ClearPortraits();

            CreatePortraits(turnManager.CurrentTurnQueue);
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
                
                portrait.Assign(unit.Render);
                portraits.Add(portrait);
            }
        }

        private void CreateDivider()
        {
            GameObject obj = Instantiate(dividerPrefab, scrollRect.content);
            TimelinePortrait portrait = obj.GetComponent<TimelinePortrait>();

            portraits.Add(portrait);
        }
    }
}
