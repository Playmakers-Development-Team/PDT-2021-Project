using Managers;
using UI.Core;
using UnityEngine;

namespace UI
{
    public class TutorialDialogue : Dialogue
    {
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

        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
    }
}
