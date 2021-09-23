using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class ExitCancelledButton : MainMenuButton
    {
        
         protected override void OnSelected()
         {
             dialogue.buttonSelected.Invoke();
             dialogue.cancelExit.Invoke();
         }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        
    }
}
