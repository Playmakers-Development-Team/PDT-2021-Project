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

    [SerializeField] private Animator animator;
    
    private static readonly int startBlurID = Animator.StringToHash("StartBlurAnim");
    private static readonly int blurAnim = Animator.StringToHash("BlurAnim");

    
    #region UIComponent
    
    protected override void Subscribe() {}

    protected override void Unsubscribe() {}

    protected override void OnComponentAwake()
    {
        animator.SetTrigger(startBlurID);
    }

    #endregion
    
    #region AnimationHandling

    private void Update() => blurImage.material.SetFloat("Amount",fill);

    public void OnStartBlurComplete() => animator.SetTrigger(blurAnim);

    #endregion

}
