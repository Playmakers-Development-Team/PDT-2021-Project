using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class ExitButtonComponent : MainMenuButton
    {
      
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.exitStarted.Invoke();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
