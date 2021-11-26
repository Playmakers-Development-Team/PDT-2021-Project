using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using TMPro;
using UI.CombatEndUI.AbilityLoadout;
using UI.CombatEndUI.AbilityUpgrading;
using UI.Commands;
using UI.Core;
using UI.Game.UnitPanels.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI
{
    public class AbilityButton : DialogueComponent<AbilityRewardDialogue>
    {
        // Must be set to true if it's one of the new abilities to choose from
        [SerializeField] protected bool isNewAbility = false;
        [SerializeField] private AbilityTooltip abilityTooltip;

        public Image AbilityRender { get; private set; }
        public String AbilityName { get; private set; }
        public String AbilityDescription { get; private set; }
        public Ability Ability { get; private set; }
        
        private Button button;
        private TextMeshProUGUI abilityNameText;
        // private TextMeshProUGUI abilityDescriptionText;

        private CommandManager commandManager;
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();

            // Get reference to ability render
            AbilityRender = GetComponentInChildren<Image>();

            // Get reference to ability name and description
            List<TextMeshProUGUI> abilityTexts = new List<TextMeshProUGUI>();
            abilityTexts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());

            foreach (var abilityText in abilityTexts)
            {
                if (abilityText.text.Equals("ABILITY NAME"))
                    abilityNameText = abilityText;
                // else
                    // abilityDescriptionText = abilityText;
            }
        }

        #endregion

        #region Drawing
        
        public void Redraw(Sprite render, Ability ability, bool renderOnly)
        {
            AbilityRender.sprite = render;
            AbilityName = ability.DisplayName;
            AbilityDescription = ability.Description;
            Ability = ability;
            
            // Used for the ability icons the player already has
            if (renderOnly)
                return;

            if (abilityTooltip != null)
                abilityTooltip.DrawAbility(ability);
            abilityNameText.text = AbilityName;
            // abilityDescriptionText.text = AbilityDescription;
        }

        public void FadeOut(float fadeOutTime)
        {
            AbilityRender.CrossFadeAlpha(0, fadeOutTime, true);
        }

        #endregion

        #region Listeners
        
        public void OnPressed()
        {
            commandManager.ExecuteCommand(new AbilitySelectedCommand(this, isNewAbility));
        }

        #endregion
        
        #region Utility Functions
        
        public void MakeSelected()
        {
            if(dialogue.GetType() == typeof(AbilityLoadoutDialogue))
            {
                if (isNewAbility)
                    dialogue.drawNewAbilityDetails.Invoke(this);
                else
                    dialogue.drawOldAbilityDetails.Invoke(this);
            }
            else if(dialogue.GetType() == typeof(AbilityUpgradeDialogue))
            {
                if (isNewAbility)
                {
                    dialogue.drawNewAbilityDetails.Invoke(this);
                    
                    // Select the ability for upgrading that the unit owns
                    // by having the code "click the button"
                    AbilityButton nonUpgradedAbility = dialogue.GetNonUpgradedAbility(AbilityName);
                    dialogue.unitSelectCanvasScript.OnAbilityButtonPress(nonUpgradedAbility);
                    // Draw for old ability panel details
                    dialogue.drawOldAbilityDetails.Invoke(nonUpgradedAbility);
                }
            }
        }
        
        public void Deselect()
        {
            // Sometimes deselect is called after the dialogue is null
            if (dialogue == null)
                return;
                
            if (dialogue.GetType() == typeof(AbilityLoadoutDialogue))
            {
                if (isNewAbility)
                    dialogue.clearNewAbilityDetails.Invoke();
                else
                    dialogue.clearOldAbilityDetails.Invoke();
            }
            else if(dialogue.GetType() == typeof(AbilityUpgradeDialogue))
            {
                if (isNewAbility)
                {
                    dialogue.clearNewAbilityDetails.Invoke();
                    
                    // Deselect the ability for upgrading that the unit owns
                    // by having the code "click the button"
                    AbilityButton nonUpgradedAbility = dialogue.GetNonUpgradedAbility(AbilityName);
                    dialogue.unitSelectCanvasScript.OnAbilityButtonPress(nonUpgradedAbility);
                    
                    // Remove info from the old ability panel details
                    dialogue.clearOldAbilityDetails.Invoke();
                    
                }
            }
        }

        #endregion
    }
}
