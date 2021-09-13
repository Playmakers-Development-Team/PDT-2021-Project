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
        private List<AbilityLoadoutDialogue.AbilityInfo> currentAbilityInfos;
        private List<AbilityLoadoutDialogue.AbilityInfo> newAbilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();
        
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
        
        internal void Redraw(TenetType newTenetType, List<AbilityLoadoutDialogue.AbilityInfo> oldAbilityInfos)
        {
            tenetType = newTenetType;
            currentAbilityInfos = oldAbilityInfos;

            newAbilityInfos = GetAbilities(3, tenetType);
            
            // Assign ability images
            for (int i = 0; i < newAbilityInfos.Count; ++i)
            {
                abilityRenders[i].sprite = newAbilityInfos[i].Render;
                abilityNames[i].text = newAbilityInfos[i].Ability.name;
                abilityDescriptions[i].text = newAbilityInfos[i].Ability.Description;
            }
        }
        
        #endregion

        #region Utility Functions

        private List<AbilityLoadoutDialogue.AbilityInfo> GetAbilities(int numberOfAbilities, TenetType tenetType)
        {
            List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();

            List<Ability> selectedAbilities = RandomiseAbilityOrder(abilityPool.PickAbilitiesByTenet(tenetType).ToList());

            for (int i = 0; i < selectedAbilities.Count; ++i)
            {
                AbilityLoadoutDialogue.AbilityInfo newAbility =
                    dialogue.GetInfo(selectedAbilities[i]);
                
                // Skip the current iteration if the character already owns the ability
                if(currentAbilityInfos.Contains(newAbility))
                    continue;
                
                abilityInfos.Add(newAbility);
                
                numberOfAbilities--;
                if (numberOfAbilities <= 0)
                    break;
            }

            if (numberOfAbilities > 0)
            {
                Debug.LogWarning("Not enough NEW abilities supplied to the ability pool " +
                                 "for the Tenet type "+tenetType+". This may result in some " +
                                 "repeated abilities");
                
                for (int i = 0; i < selectedAbilities.Count; ++i)
                {
                    AbilityLoadoutDialogue.AbilityInfo newAbility =
                        dialogue.GetInfo(selectedAbilities[i]);

                    abilityInfos.Add(newAbility);
                
                    numberOfAbilities--;
                    if (numberOfAbilities <= 0)
                        break;
                }
            }
            
            return abilityInfos;
        }

        private List<Ability> RandomiseAbilityOrder(List<Ability> abilityList)
        {
            var count = abilityList.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var rand = Random.Range(i, count);
                var temp = abilityList[i];
                abilityList[i] = abilityList[rand];
                abilityList[rand] = temp;
            }

            return abilityList;
        }

        #endregion
    }
}
