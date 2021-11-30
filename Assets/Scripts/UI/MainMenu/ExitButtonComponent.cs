namespace UI.MainMenu
{
    public class ExitButtonComponent : MainMenuButton
    {

        #region ButtonHandling
        
        protected override void OnSelected()
        {
            base.OnSelected();
            
            dialogue.buttonSelected.Invoke();
            dialogue.exitStarted.Invoke();
        }

        #endregion
           
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
    }
}
