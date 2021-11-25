using System.Collections;
using System.Collections.Generic;
using UI.Core;
using UI.PauseScreen;
using UnityEngine;

public class ContinueButtonComponent : PauseScreenButton
{
    #region UIComponent
    
    protected override void Subscribe() {}

    protected override void Unsubscribe() {}
    
    #endregion

    protected override void OnSelected()
    {
        dialogue.buttonSelected.Invoke();
        dialogue.continueGame.Invoke();
    }
}
