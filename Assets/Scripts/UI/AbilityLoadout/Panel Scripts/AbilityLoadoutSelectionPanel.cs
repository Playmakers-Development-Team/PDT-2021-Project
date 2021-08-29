using System.Collections.Generic;
using System.Linq;
using Abilities;
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
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        protected override void OnComponentAwake()
        {
            foreach (var ability in abilityButtons)
            {
                abilityRenders.Add(ability.GetComponentInChildren<Image>());
                abilityNames.Add(ability.GetComponentInChildren<TextMeshProUGUI>());
            }
        }

        #endregion

        #region Listeners

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
