using System.Collections.Generic;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Abilities
{
    public class UnitAbilitiesCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        // Placeholder selection sprite and sprite offset
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        // Used to identify the currently selected ability
        private AbilityButton currentSelectedAbility;
        
        // Stores the data for the current list of abilities
        protected internal List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos;
        
        // References the ability button scripts
        [SerializeField] protected List<AbilityButton> abilityButtons = new List<AbilityButton>();
        private List<Button> unityAbilityButtons = new List<Button>();

        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            // Assign unity buttons from scripts
            foreach (var abilityButton in abilityButtons)
                unityAbilityButtons.Add(abilityButton.GetComponent<Button>());
        }
        
        #endregion
        
        #region Listeners
        
        public void OnAbilityButtonPress(AbilityButton abilityButton)
        {
            if (currentSelectedAbility == abilityButton)
            {
                currentSelectedAbility.Deselect();
                
                // Make no ability selected
                currentSelectedAbility = null;
                
                // Turn Off Visual Placeholder
                selectedAbilityImage.enabled = false;
            }
            else
            {
                // Deselect the old ability (if there was one)
                if(currentSelectedAbility != null)
                    currentSelectedAbility.Deselect();
                
                // Select the new ability
                currentSelectedAbility = abilityButton;
                currentSelectedAbility.MakeSelected();

                // Visual Placeholder
                selectedAbilityImage.enabled = true;
                selectedAbilityImage.gameObject.transform.position = abilityButton.transform.position + selectedOffset;
            }
        }
        
        #endregion

        #region Utility Functions
        
        public void RemoveSelectedAbility(IUnit unit)
        {
            foreach (var ability in unit.Abilities)
            {
                if (ability.DisplayName.Equals(currentSelectedAbility.AbilityName))
                {
                    unit.Abilities.Remove(ability);
                    break;
                }
            }
        }

        public void EnableAbilityButtons()
        {
            foreach (var unityAbilityButton in unityAbilityButtons)
                unityAbilityButton.enabled = true;
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<AbilityLoadoutDialogue.AbilityInfo> newAbilityInfo)
        {
            // Assign unit info
            abilityInfos = newAbilityInfo;
            
            if (abilityInfos == null)
                return;

            // Assign ability images
            for (int i = 0; i < abilityInfos.Count; ++i)
            {
                abilityButtons[i].Redraw(abilityInfos[i].Render,
                    abilityInfos[i].Ability.DisplayName,
                    abilityInfos[i].Ability.Description,
                    true);
            }
        }

        #endregion
    }
}
