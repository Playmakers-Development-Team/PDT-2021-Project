using Game;
using Managers;

namespace UI.MainMenu
{
    public class ContinueGameButtonComponent : MainMenuButton
    {
        private GameManager gameManager;
        
        #region DialogueComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            
            gameManager = ManagerLocator.Get<GameManager>();
            
            // Hide the button if no save game exists
            if (!GameManager.HasSavedGame())
                gameObject.SetActive(false);
        }
        
        #endregion
        
        #region PanelButton

        protected override void OnSelected()
        {
            dialogue.buttonSelected.Invoke();
            dialogue.gameContinued.Invoke();
        }
        
        #endregion
    }
}
