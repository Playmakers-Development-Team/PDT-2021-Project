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
        private Image abilityRender;
        private TextMeshProUGUI abilityName;
        private TextMeshProUGUI abilityDescription;

        private CommandManager commandManager;
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        // Performs it's own awake to avoid race conditions
        public void AbilityButtonAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            
            // Get reference to ability render
            abilityRender = GetComponentInChildren<Image>();

            // Get reference to ability name and description
            List<TextMeshProUGUI> abilityTexts = new List<TextMeshProUGUI>();
            abilityTexts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());

            foreach (var abilityText in abilityTexts)
            {
                if (abilityText.text.Equals("ABILITY NAME"))
                    abilityName = abilityText;
                else
                    abilityDescription = abilityText;
            }
        }
        
        #endregion

        #region Drawing

        public void Redraw(Sprite render, string name, string description)
        {
            abilityRender.sprite = render;
            abilityName.text = name;
            abilityDescription.text = description;
        }

        #endregion

        #region Listeners
        
        public void OnPressed()
        {
            commandManager.ExecuteCommand(new AbilitySelectedCommand(this));
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
