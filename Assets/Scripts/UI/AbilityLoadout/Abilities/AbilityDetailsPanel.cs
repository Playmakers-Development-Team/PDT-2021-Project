using System.Collections.Generic;
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
            HideValues();
        }

        #endregion

        #region Drawing

        public void Redraw(AbilityButton abilityButton)
        {
            abilityRender.enabled = true;
            
            if (abilityButton.AbilityName == null)
            {
                HideValues();
                abilityName.text = "OPEN SLOT";
            }
            else
            {
                abilityRender.sprite = abilityButton.AbilityRender.sprite;
                abilityName.text = abilityButton.AbilityName.text;
                abilityDescription.text = abilityButton.AbilityDescription.text;
            }
        }

        #endregion

        #region Utility Functions

        private void HideValues()
        {
            abilityRender.enabled = false;
            abilityName.text = "";
            abilityDescription.text = "";
        }

        #endregion
    }
}
