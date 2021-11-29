namespace UI.MainMenu
{
    public class SettingsButtonComponent : MainMenuButton
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
            dialogue.settingConfirmed.Invoke();
        }
        
        #endregion
    }
}
