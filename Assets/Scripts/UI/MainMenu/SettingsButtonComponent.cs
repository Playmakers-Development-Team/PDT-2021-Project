namespace UI.MainMenu
{
    public class SettingsButtonComponent : MainMenuButton
    {
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.settingConfirmed.Invoke();
        }
    }
}
