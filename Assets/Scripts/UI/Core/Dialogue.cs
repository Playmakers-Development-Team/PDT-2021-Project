using Managers;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// A Dialogue is a collection of <see cref="DialogueComponent{T}"/>s that should be presented and interacted with at the same time.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Dialogue : MonoBehaviour
    {
        internal readonly Event promoted = new Event();
        internal readonly Event demoted = new Event();
        internal readonly Event closed = new Event();
        
        protected UIManager manager;
        protected CanvasGroup canvasGroup;
        
        
        #region MonoBehaviour

        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            canvasGroup = GetComponent<CanvasGroup>();
            
            foreach (Canvas canvas in GetComponentsInChildren<Canvas>())
                canvas.worldCamera = Camera.main;

            OnAwake();
            
            manager.Add(this);
        }

        #endregion
        
        
        #region Dialogue

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
        
        internal void Close()
        {
            OnClose();
            Destroy(gameObject);
            closed.Invoke();
        }

        internal virtual void OnAwake() {}


        protected abstract void OnClose();

        protected abstract void OnPromote();

        protected abstract void OnDemote();
        
        #endregion
    }
}
