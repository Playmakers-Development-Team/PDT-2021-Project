using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Abilities
{
    public class AbilityDetailsPanel : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private Image abilityRender;
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private TextMeshProUGUI abilityDescription;
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            ClearValues();
        }

        #endregion

        #region Drawing

        public void Redraw(AbilityButton abilityButton)
        {
            abilityRender.enabled = true;
            
            if (abilityButton.AbilityName == null)
            {
                ClearValues();
                abilityName.text = "OPEN SLOT";
            }
            else
            {
                abilityRender.sprite = abilityButton.AbilityRender.sprite;
                abilityName.text = abilityButton.AbilityName;
                abilityDescription.text = abilityButton.AbilityDescription;
            }
        }

        #endregion

        #region Utility Functions

        public void ClearValues()
        {
            abilityRender.enabled = false;
            abilityName.text = "";
            abilityDescription.text = "";
        }

        #endregion
    }
}
