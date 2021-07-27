using Managers;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// A UIComponent is any UI element that exists as part of a <see cref="Dialogue"/>. All UI Elements should extend this class. 
    /// </summary>
    /// <typeparam name="T">The <see cref="Dialogue"/> type this UIComponent is a part of.</typeparam>
    public abstract class DialogueComponent<T> : MonoBehaviour where T : Dialogue
    {
        protected UIManager manager;
        protected T dialogue;
        
        
        #region MonoBehaviour
        
        private void Awake()
        {
            manager = ManagerLocator.Get<UIManager>();
            OnComponentAwake();
        }

        private void OnEnable()
        {
            dialogue = manager.GetDialogue<T>();
            
            if (dialogue)
                Subscribe();
            else
                manager.dialogueAdded.AddListener(OnDialogueAdded);
            
            OnComponentEnabled();
        }

        private void OnDisable()
        {
            OnComponentDisabled();
            Unsubscribe();
            
            dialogue = null;
        }

        private void Start()
        {
            OnComponentStart();
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

        protected virtual void OnComponentStart() {}

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();
        
        #endregion
    }
}
