using UI.Core;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.MainMenu.ExitConfirmation
{
    public class ExitConfirmationDialogue : Dialogue
    {
        internal readonly Event buttonSelected = new Event();
        internal readonly Event cancel = new Event();
        internal readonly Event confirm = new Event();
            
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();
            
            cancel.AddListener(() =>
            {
                manager.Pop();
            });
            
            confirm.AddListener(() =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });
        }
    }
}