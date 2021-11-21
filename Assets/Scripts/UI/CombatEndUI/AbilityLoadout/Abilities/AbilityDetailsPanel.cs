using System.Collections.Generic;
using TMPro;
using UI.Core;
using UI.Game.UnitPanels.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.AbilityLoadout.Abilities
{
    public class AbilityDetailsPanel : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private Image abilityRender;
        [SerializeField] private AbilityTooltip abilityTooltip;
        
        public TextMeshProUGUI abilityName { get; private set; }
        private TextMeshProUGUI abilityDescription;
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            // Get reference to ability name and description
            List<TextMeshProUGUI> abilityTexts = new List<TextMeshProUGUI>();
            abilityTexts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());

            foreach (var abilityText in abilityTexts)
            {
                if (abilityText.text.Equals("ABILITY NAME"))
                    abilityName = abilityText;
                // else
                    // abilityDescription = abilityText;
            }
            
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
                // abilityDescription.text = abilityButton.AbilityDescription;
                
                if (abilityTooltip != null)
                    abilityTooltip.DrawAbility(abilityButton.Ability);
            }
        }

        #endregion

        #region Utility Functions

        public void ClearValues()
        {
            abilityRender.enabled = false;
            abilityName.text = "";
            // abilityDescription.text = "";
            
            if (abilityTooltip != null)
                abilityTooltip.HideAbilities();
        }

        #endregion
    }
}
