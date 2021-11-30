namespace UI.MainMenu.ExitConfirmation
{
    public class ExitConfirmationCancelButton : ExitConfirmationButton
    {
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.cancel.Invoke();
        }
    }
}
