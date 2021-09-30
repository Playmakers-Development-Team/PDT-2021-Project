using UI.Core;

namespace UI.PauseScreen
{
    public class CancelExitButtonComponent : PauseScreenButton
    {
        #region UIComponent
    
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    
        #endregion

        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.cancelExit.Invoke();
        }
    }
}
