using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SplashScreen
{
    public class SplashScreenBackground : DialogueComponent<SplashScreenDialogue>
    {
        [SerializeField] private Image backgroundImage;
        
        #region UIComponent
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
        
        #region ImageHandling
        public void SetImage(Sprite sprite)
        {
            backgroundImage.sprite = sprite;
        }
        
        #endregion

    }
}
