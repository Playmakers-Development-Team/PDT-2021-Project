namespace UI.MainMenu.ExitConfirmation
{
    public class ExitConfirmationConfirmButton : ExitConfirmationButton
    {
        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.confirm.Invoke();
        }
    }
}
