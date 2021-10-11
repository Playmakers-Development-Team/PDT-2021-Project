using UI.Core;
using UI.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.MainMenu
{ 
    public class MainMenuInput : DialogueComponent<MainMenuDialogue>
    {
        

        private PlayerControls controls;
        
        #region InputDelegates
         
        private void OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed  || !dialogue.isOnSplashScreen)
                return;

            dialogue.splashScreenEnded.Invoke();
        }
        
        #endregion

        
        #region OverrideFunctions
        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            controls = new PlayerControls();
            controls.UI.AnyButton.performed += OnClick;
        }
        
        protected override void OnComponentEnabled()
        {
            base.OnComponentEnabled();
            controls.Enable();
            
        }

        protected override void OnComponentDisabled()
        {
            base.OnComponentDisabled();
            controls.Disable();
        }
        
        #endregion

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
