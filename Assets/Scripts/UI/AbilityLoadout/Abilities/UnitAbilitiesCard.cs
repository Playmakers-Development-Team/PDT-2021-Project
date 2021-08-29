using System.Collections.Generic;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Abilities
{
    public class UnitAbilitiesCard : DialogueComponent<AbilityLoadoutDialogue>
    {
        protected internal List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos;

        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        [SerializeField] protected List<Button> abilityButtons = new List<Button>();
        private List<Image> abilityRenders = new List<Image>();
        

        #region UIComponent

        protected override void OnComponentAwake()
        {
            foreach (var abilityButton in abilityButtons)
                abilityRenders.Add(abilityButton.GetComponent<Image>());
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
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

        #region Utility Functions

        public void EnableAbilityButtons()
        {
            foreach (var abilityButton in abilityButtons)
                abilityButton.enabled = true;
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
                abilityRenders[i].sprite = abilityInfos[i].Render;
            }
        }

        #endregion
    }
}
