using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.General
{
    public class OpenButton : DialogueComponent<Dialogue>
    {
        [SerializeField] private GameObject dialoguePrefab;
        [SerializeField] private Button button;


        #region UIComponent

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnPressed);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnPressed);
        }
        
        #endregion
        
        
        #region Listeners

        private void OnPressed()
        {
            Instantiate(dialoguePrefab, dialogue.transform.parent);
        }
        
        #endregion
    }
}
