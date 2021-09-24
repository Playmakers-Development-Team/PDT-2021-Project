using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class ExitGameButtonComponent : MainMenuButton
    {
        
        #region UIComponent
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion

        #region ButtonHandling
        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.exitConfirmed.Invoke();
        }
        
        #endregion
    }
}
