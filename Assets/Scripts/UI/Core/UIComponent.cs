using Managers;
using UnityEngine;

namespace UI.Core
{
    public abstract class UIComponent<T> : MonoBehaviour where T : Dialogue
    {
        protected UIManager manager;
        protected T dialogue;
        
        
        #region MonoBehaviour
        
        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            manager.dialogueAdded.AddListener(OnDialogueAdded);
            OnComponentAwake();
        }

        private void OnEnable()
        {
            if (!dialogue)
                dialogue = manager.GetDialogue<T>();
            
            if (dialogue)
                Subscribe();
            
            OnComponentEnabled();
        }

        private void OnDisable()
        {
            OnComponentDisabled();
            Unsubscribe();
            
            dialogue = null;
        }

        #endregion
        
        
        #region UIComponent
        
        private void OnDialogueAdded(Dialogue addedDialogue)
        {
            if (!(addedDialogue is T compatible))
                return;

            manager.dialogueAdded.RemoveListener(OnDialogueAdded);
            dialogue = compatible;

            Subscribe();
        }
        
        protected virtual void OnComponentAwake() {}

        protected virtual void OnComponentEnabled() {}

        protected virtual void OnComponentDisabled() {}

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();
        
        #endregion
    }
}
