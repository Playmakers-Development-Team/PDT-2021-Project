using System;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Tutorial
{
    public class TutorialDialogue : Dialogue
    {
        [SerializeField] private TutorialDialogue nextTutorial;
        
        private UIManager uiManager;
        
        protected override void OnDialogueAwake()
        {
            uiManager = ManagerLocator.Get<UIManager>();
            
            // Add to UI manager and show starting panel
            uiManager.Add(this, false);
        }

        public void CloseTutorial()
        {
            Close();
        }

        protected override void OnClose()
        {
            if (nextTutorial != null)
            {
                Debug.Log($"Loading next tutorial '{nextTutorial.name}'");
                TutorialDialogue nextTutorialDialogue = Instantiate(nextTutorial, transform.parent); 
                uiManager.Add(nextTutorialDialogue, false);
            }
        }

        protected override void OnPromote() {}

        protected override void OnDemote() {}
    }
}
