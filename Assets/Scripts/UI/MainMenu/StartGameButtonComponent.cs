namespace UI.MainMenu
{
    public class StartGameButtonComponent : MainMenuButton
    {
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.gameStarted.Invoke();
        }
    }
}
