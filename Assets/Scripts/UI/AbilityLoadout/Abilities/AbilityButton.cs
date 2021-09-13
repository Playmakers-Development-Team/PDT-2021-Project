using System;
using System.Collections.Generic;
using Commands;
using Managers;
using TMPro;
using UI.Commands;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Abilities
{
    public class AbilityButton : DialogueComponent<AbilityLoadoutDialogue>
    {
        // Must be set to true if it's one of the new abilities to choose from
        [SerializeField] private bool isNewAbility = false;
        
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
            // Insert selected visual stuff here
        }
        
        public void Deselect()
        {
            // Remove selected visual stuff here
        }
        
        #endregion
    }
}
