using UI;
using UI.Core;
using UI.PauseScreen;
using UnityEngine;
using UnityEngine.UI;

public class BlurComponent : DialogueComponent<PauseScreenDialogue>
{
    [SerializeField] private Image blurImage;
    
    [SerializeField, Range(0f, 0.045f)]
    private float fill;
    
    #region UIComponent
    
    protected override void Subscribe() {}

    protected override void Unsubscribe() {}
    
    #endregion
    
    #region AnimationHandling

    private void Update() => blurImage.material.SetFloat("Amount",fill);
    
    
    #endregion

}
