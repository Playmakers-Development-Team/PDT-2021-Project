namespace UI.MainMenu
{
    public class CreditsButtonComponent : MainMenuButton
    {
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region ButtonHandling
        
        protected override void OnSelected()
        {
            base.OnSelected();
            
            dialogue.buttonSelected.Invoke();
            dialogue.creditsConfirmed.Invoke();
        }
        
        #endregion
    }
}
