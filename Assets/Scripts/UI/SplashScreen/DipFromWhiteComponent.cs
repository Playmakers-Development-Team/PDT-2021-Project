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

        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion

        #region AnimationHandling

        public async void Begin() => await StartAnimation();

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            animator.enabled = false;
        }

        private async UniTask StartAnimation()
        {
            await UniTask.Delay(timeTillFade);
            animator.enabled = true;
        }

        public void CompleteAnimation()
        {
            animator.enabled = false;
            image.color = new Color(255, 255, 255, 255);
        }

        #endregion

        #region Boolean Getters

        public bool IsAnimationPlaying()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                return false;

            return true;

        }
        
        #endregion
    }
}
