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
        
        private Button button;
        public Image AbilityRender { get; private set; }
        public TextMeshProUGUI AbilityName { get; private set; }
        public TextMeshProUGUI AbilityDescription { get; private set; }

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
                    AbilityName = abilityText;
                else
                    AbilityDescription = abilityText;
            }
        }
        
        #endregion

        #region Drawing

        // Render only version (current abilities)
        public void Redraw(Sprite render)
        {
            AbilityRender.sprite = render;
        }
        
        // Full info version (new abilities)
        public void Redraw(Sprite render, string name, string description)
        {
            AbilityRender.sprite = render;
            AbilityName.text = name;
            AbilityDescription.text = description;
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
