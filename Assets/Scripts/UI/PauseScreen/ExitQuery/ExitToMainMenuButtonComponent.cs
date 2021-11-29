namespace UI.PauseScreen.ExitQuery
{
    public class ExitToMainMenuButtonComponent : ExitQueryButton
    {
        #region UIComponent
    
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    
        #endregion

        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.exitToMainMenu.Invoke();
        }
    }
}
