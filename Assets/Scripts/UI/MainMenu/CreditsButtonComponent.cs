namespace UI.MainMenu
{
    public class CreditsButtonComponent : MainMenuButton
    {
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.creditsConfirmed.Invoke();
        }
    }
}
