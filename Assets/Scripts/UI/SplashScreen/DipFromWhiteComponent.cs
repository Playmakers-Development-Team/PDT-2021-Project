using Cysharp.Threading.Tasks;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SplashScreen
{
    public class DipFromWhiteComponent : DialogueComponent<SplashScreenDialogue>
    {
        [Tooltip("Amount of time until the fade begins in milliseconds")] [SerializeField]
        private int timeTillFade;

        [SerializeField] private Animator animator;
        [SerializeField] private Image image;
        
        #region Properties
        
        public bool IsAnimating { get; private set; }
        
        #endregion
        
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion

        #region AnimationHandling

        public async void Begin() => await StartAnimation();

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            IsAnimating = true;
            animator.enabled = false;
        }

        public void AnimationStarted() => IsAnimating = true;
        
        public void FadeCompleted() => IsAnimating = false;

        private async UniTask StartAnimation()
        {
            
            await UniTask.Delay(timeTillFade);
            
            if (IsAnimating)
                animator.enabled = true;
        }

        public void CompleteAnimation()
        {
            IsAnimating = false;
            animator.enabled = false;
            image.color = new Color(255, 255, 255, 0);
        }

        #endregion
        
    }
}
