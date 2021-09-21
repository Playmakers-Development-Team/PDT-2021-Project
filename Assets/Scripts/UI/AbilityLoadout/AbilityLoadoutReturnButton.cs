using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout
{
    public class AbilityLoadoutReturnButton : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private Button returnButton;

        #region UIComponent

        protected override void Subscribe()
        {
            returnButton.interactable = true;
            
            // Listen to Events
            dialogue.abilitySwap.AddListener(() =>
            {
                returnButton.interactable = false;
            });
        }

        protected override void Unsubscribe()
        {
            // Remove listening Events
            dialogue.abilitySwap.RemoveListener(() =>
            {
                returnButton.interactable = false;
            });
        }
        
        #endregion
        
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.showUnitSelectPanel.Invoke();
        }
        
        #endregion
    }
}
