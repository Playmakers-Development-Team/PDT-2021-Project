using System;
using System.Collections.Generic;
using Commands;
using Managers;
using TMPro;
using UI.CombatEndUI.AbilityLoadout;
using UI.CombatEndUI.AbilityUpgrading;
using UI.Commands;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI
{
    public class AbilityButton : DialogueComponent<AbilityRewardDialogue>
    {
        // Must be set to true if it's one of the new abilities to choose from
        [SerializeField] protected bool isNewAbility = false;

        public Image AbilityRender { get; private set; }
        public String AbilityName { get; private set; }
        public String AbilityDescription { get; private set; }
        
        private Button button;
        private TextMeshProUGUI abilityNameText;
        private TextMeshProUGUI abilityDescriptionText;

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
                else
                    abilityDescriptionText = abilityText;
            }
        }

        #endregion

        #region Drawing
        
        public void Redraw(Sprite render, string name, string description, bool renderOnly)
        {
            AbilityRender.sprite = render;
            AbilityName = name;
            AbilityDescription = description;
            
            // Used for the ability icons the player already has
            if (renderOnly)
                return;

            abilityNameText.text = AbilityName;
            abilityDescriptionText.text = AbilityDescription;
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
                    
                    dialogue.drawOldAbilityDetails.Invoke(GetNonUpgradedAbility());
                }
            }
        }
        
        public void Deselect()
        {
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
                    dialogue.clearOldAbilityDetails.Invoke();
                }
            }
        }
        
        // Search through the selected unit's abilities to find the NonUpgraded ability
        private AbilityButton GetNonUpgradedAbility()
        {
            List<AbilityButton> currentAbilityButtons = dialogue.unitSelectCanvasScript.abilitiesCards[0].abilityButtons;
            
            foreach (AbilityButton nonUpgradedAbility in currentAbilityButtons)
            {
                if (this.AbilityName.Equals(nonUpgradedAbility.AbilityName + "+"))
                {
                    dialogue.unitSelectCanvasScript.OnAbilityButtonPress(nonUpgradedAbility);
                    return nonUpgradedAbility;
                }
            }
            
            Debug.LogError("Could not find NonUpgraded ability for " + AbilityName);

            return null;
        }

        #endregion
    }
}
