using System.Collections.Generic;
using UI.Core;
using UnityEngine;

namespace UI.SplashScreen
{
    public class SplashScreenDialogue : Dialogue
    {

        [SerializeField] private SplashScreenBackground backgroundImage;
        [SerializeField] private List<Sprite> backgroundImages = new List<Sprite>();
        [SerializeField] private DipFromWhiteComponent splashScreenDip;

        
        #region OverrideFunctions
        protected override void OnClose() {}

        protected override void OnPromote()
        {
            canvasGroup.interactable = true;
        }

        protected override void OnDemote()
        {
            canvasGroup.interactable = false;
        }
        
        #endregion
        
        #region SplashScreenHandling

        protected override void OnDialogueAwake()
        {
            SetRandomBackgroundImage();
            splashScreenDip.Begin();
        }

        private void SetRandomBackgroundImage() =>
            backgroundImage.SetImage(backgroundImages[Random.Range(0, 3)]);

        #endregion
    }

}
