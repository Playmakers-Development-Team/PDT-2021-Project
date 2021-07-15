using Managers;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Dialogue : MonoBehaviour
    {
        internal readonly Event closed = new Event();
        internal readonly Event promoted = new Event();
        internal readonly Event demoted = new Event();
        
        protected UIManager manager;
        protected CanvasGroup canvasGroup;
        
        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            OnAwake();
        }

        private void Start()
        {
            manager.Add(this);
        }

        internal void Close()
        {
            OnClose();
            Destroy(gameObject);
            closed.Invoke();
        }

        internal void Promote()
        {
            canvasGroup.interactable = true;
            
            OnPromote();
            promoted.Invoke();
        }

        internal void Demote()
        {
            canvasGroup.interactable = false;
            
            OnDemote();
            demoted.Invoke();
        }


        internal virtual void OnAwake() {}


        protected abstract void OnClose();

        protected abstract void OnPromote();

        protected abstract void OnDemote();
    }
}
