using UI.Core;
using UnityEngine;

namespace UI.General
{
    public class OpenButton : UIComponent<Dialogue>
    {
        [SerializeField] private GameObject dialoguePrefab;


        #region UIComponent
        
        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }
        
        #endregion
        
        
        #region Listeners

        public void OnPressed()
        {
            Instantiate(dialoguePrefab, dialogue.transform.parent);
        }
        
        #endregion
    }
}
