using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class ExitGameButtonComponent : MainMenuButton
    {
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.exitConfirmed.Invoke();
        }
    }
}
