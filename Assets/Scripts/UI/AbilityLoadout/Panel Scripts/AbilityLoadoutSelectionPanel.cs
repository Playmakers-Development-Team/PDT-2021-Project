using System.Collections.Generic;
using System.Linq;
using Abilities;
using Cysharp.Threading.Tasks.Triggers;
using TenetStatuses;
using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Panel_Scripts
{
    public class AbilityLoadoutSelectionPanel : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        [SerializeField] private AbilityPool abilityPool;
        
        private TenetType tenetType;
        
        private List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();
        
        [SerializeField] protected List<Button> abilityButtons = new List<Button>();
        private List<Image> abilityRenders = new List<Image>();
        private List<TextMeshProUGUI> abilityNames = new List<TextMeshProUGUI>();
        private List<TextMeshProUGUI> abilityDescriptions = new List<TextMeshProUGUI>();
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        protected override void OnComponentAwake()
        {
            foreach (var ability in abilityButtons)
            {
                // Get reference to ability renders
                abilityRenders.Add(ability.GetComponentInChildren<Image>());

                // Get reference to ability names and descriptions
                List<TextMeshProUGUI> abilityTexts = new List<TextMeshProUGUI>();
                abilityTexts.AddRange(ability.GetComponentsInChildren<TextMeshProUGUI>());

                foreach (var abilityText in abilityTexts)
                {
                    if(abilityText.text.Equals("ABILITY NAME"))
                        abilityNames.Add(abilityText);
                    else
                        abilityDescriptions.Add(abilityText);
                }
            }
        }

        #endregion

        #region Listeners

        // This is for the yellow circle that shows an ability is selected
        public void OnPressed(GameObject selectedAbilityButton)
        {
            if (selectedAbilityImage.enabled && 
                selectedAbilityImage.gameObject.transform.position == selectedAbilityButton.transform.position + selectedOffset)
            {
                selectedAbilityImage.enabled = false;
            }
            else
            {
                selectedAbilityImage.enabled = true;
                selectedAbilityImage.gameObject.transform.position = selectedAbilityButton.transform.position + selectedOffset;
            }
        }

        #endregion
        
        #region Drawing
        
        internal void Redraw(TenetType newTenetType)
        {
            tenetType = newTenetType;

            abilityInfos = GetRandomAbilities(3, tenetType);
            
            // Assign ability images
            for (int i = 0; i < abilityInfos.Count; ++i)
            {
                abilityRenders[i].sprite = abilityInfos[i].Render;
                abilityNames[i].text = abilityInfos[i].Ability.name;
                abilityDescriptions[i].text = abilityInfos[i].Ability.Description;
            }
        }
        
        #endregion

        #region Utility Functions

        private List<AbilityLoadoutDialogue.AbilityInfo> GetRandomAbilities(int numberOfAbilities, TenetType tenetType)
        {
            List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();

            List<Ability> selectedAbilities = abilityPool.PickAbilitiesByTenet(tenetType).ToList();
            
            for (int i = 0; i < selectedAbilities.Count; ++i)
            {
                abilityInfos.Add(dialogue.GetInfo(selectedAbilities[i]));
                
                numberOfAbilities--;
                if (numberOfAbilities <= 0)
                    break;
            }

            return abilityInfos;
        }

        #endregion
    }
}
