namespace UI.MainMenu.ExitConfirmation
{
    // TODO: Not technically inheriting from the right class.
    public class ExitCancelledButton : MainMenuButton
    {
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
        
        #region ButtonHandling

        protected override void OnSelected()
        {
            manager.Pop();
        }

        #endregion
    }
}
