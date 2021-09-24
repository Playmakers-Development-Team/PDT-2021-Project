using UI.Core;
using UI.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI.SplashScreen
{
    public class SplashScreenInput : DialogueComponent<SplashScreenDialogue>
    {

        [SerializeField] private DipFromWhiteComponent dipFromWhiteComponent;
        
         private PlayerControls controls;

         #region InputDelegates
         
        private void OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            
            if (dipFromWhiteComponent.IsAnimating)
            {
                dipFromWhiteComponent.CompleteAnimation();
                return;
            }
            
            //TODO: TEMP CODE SHOULD BE CHANGED TO THE APPROPRIATE SYSTEM FOR LOADING NEW SCENES AND BE DIRECTED TO THE MAIN MENU SCENE
            SceneManager.LoadScene("Scenes/Design/Playtest Beta Map");
        }
        
        #endregion

        
        #region OverrideFunctions
        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            controls = new PlayerControls();
            controls.UI.AnyKey.performed += OnClick;

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
        
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
    }
}
