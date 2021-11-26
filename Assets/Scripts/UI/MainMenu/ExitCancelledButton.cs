using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class ExitCancelledButton : MainMenuButton
    {
        
        #region ButtonHandling
        
         protected override void OnSelected()
         {
             base.OnSelected();
             
             dialogue.buttonSelected.Invoke();
             dialogue.cancelExit.Invoke();
         }
        
         #endregion
            
         #region UIComponent
         protected override void Subscribe() {}

         protected override void Unsubscribe() {}
        
         #endregion
        
        
    }
}
