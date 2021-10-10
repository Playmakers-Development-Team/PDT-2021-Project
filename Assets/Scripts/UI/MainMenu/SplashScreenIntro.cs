using System.Collections;
using System.Collections.Generic;
using UI.Core;
using UI.MainMenu;
using UnityEngine;

public class SplashScreenIntro : DialogueComponent<MainMenuDialogue>
{
    #region UIComponent

    [SerializeField] private Animator gameTitleAnimator;
    
    protected override void Subscribe() {}

    protected override void Unsubscribe() {}
    
    #endregion

    protected override void OnComponentAwake()
    {
        
    }
}
