using System.Collections.Generic;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.AbilityLoadout.Abilities
{
    public class UnitAbilitiesCard : DialogueComponent<AbilityRewardDialogue>
    {
        // Placeholder selection sprite and sprite offset
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        // Used to identify the currently selected ability
        private AbilityButton currentSelectedAbility;
        
        // Stores the data for the current list of abilities
        protected internal List<LoadoutAbilityInfo> abilityInfos;
        
        // References the ability button scripts
        [SerializeField] protected internal List<AbilityButton> abilityButtons = new List<AbilityButton>();
        private List<Button> unitAbilityButtons = new List<Button>();
        
        // Used to slide into the correct position when selected
        private RectTransform rectTransform;
        internal bool isSliding = false;

        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            // Assign unity buttons from scripts
            foreach (var abilityButton in abilityButtons)
                unitAbilityButtons.Add(abilityButton.GetComponent<Button>());
        }
        
        #endregion
        
        #region Monobehavior Events

        private void Update()
        {
            if (isSliding)
            {
                // Move our position a step closer to the target.
                float step =  dialogue.selectedUnitSlideSpeed * Time.deltaTime; // calculate distance to move
                rectTransform.anchoredPosition -= new Vector2(step, 0);

                // Check if the position of the unit card and final position are approximately equal.
                if (rectTransform.anchoredPosition.x - dialogue.selectedUnitPosition < 0.001f)
                {
                    // Set the final position
                    rectTransform.anchoredPosition = new Vector2(dialogue.selectedUnitPosition, rectTransform.anchoredPosition.y);
                    
                    isSliding = false;
                }
            }
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
        
        public void RemoveSelectedAbility(LoadoutUnitInfo unitInfo)
        {
            foreach (var abilityInfo in unitInfo.AbilityInfo)
            {
                if (abilityInfo.Ability.name.Equals(currentSelectedAbility.AbilityName))
                {
                    unitInfo.AbilityInfo.Remove(abilityInfo);
                    unitInfo.Unit.Abilities.Remove(abilityInfo.Ability);
                    break;
                }
            }
        }

        public void EnableAbilityButtons()
        {
            foreach (var unityAbilityButton in unitAbilityButtons)
                unityAbilityButton.enabled = true;
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<LoadoutAbilityInfo> newAbilityInfo)
        {
            // Assign unit info
            abilityInfos = newAbilityInfo;
            
            if (abilityInfos == null)
                return;

            // Assign ability images
            for (int i = 0; i < abilityInfos.Count; ++i)
            {
                abilityButtons[i].Redraw(abilityInfos[i].Render, abilityInfos[i].Ability, true);
            }
        }
        
        internal void FadeOut(float fadeOutTime)
        {
            foreach (var abilityButton in abilityButtons)
            {
                abilityButton.FadeOut(fadeOutTime);
            }
        }

        #endregion
    }
}
