namespace UI.PauseScreen
{
    public class ExitGameButton : PauseScreenButton
    {
   
        #region UIComponent
    
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    
        #endregion

        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.exitGame.Invoke();
        }
    }
}
