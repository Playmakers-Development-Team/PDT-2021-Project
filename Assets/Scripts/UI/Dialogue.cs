using Managers;
using UnityEngine;

namespace UI
{
    /*
     * Showing a dialogue
     * Hiding a dialogue
     * Disabling a dialogue
     * Enabling a dialogue
     */
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Dialogue : MonoBehaviour
    {
        internal readonly Event hidden = new Event();
        internal readonly Event promoted = new Event();
        internal readonly Event demoted = new Event();
        
        private UIManager manager;
        private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            manager.Add(this);
        }

        internal void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.0f;

            OnHide();
            hidden.Invoke();
        }

        internal void Promote()
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1.0f;
            
            OnPromote();
            promoted.Invoke();
        }

        internal void Demote()
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
            
            OnDemote();
            demoted.Invoke();
        }


        protected abstract void OnHide();

        protected abstract void OnPromote();

        protected abstract void OnDemote();
    }
}
