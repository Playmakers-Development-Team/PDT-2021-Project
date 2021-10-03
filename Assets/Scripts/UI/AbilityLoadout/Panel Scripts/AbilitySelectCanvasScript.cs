using System.Collections.Generic;
using System.Linq;
using Abilities;
using Managers;
using TenetStatuses;
using UI.AbilityLoadout.Abilities;
using UI.Core;
using Units;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.AbilityLoadout.Panel_Scripts
{
    public class AbilitySelectCanvasScript : DialogueComponent<AbilityLoadoutDialogue>
    {
        // Placeholder selection sprite and sprite offset
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        // Used to identify the currently selected new ability
        private AbilityButton currentSelectedAbility;
        
        // Holds data for all abilities, used to obtain new abilities
        [SerializeField] private AbilityPool abilityPool;
        
        // Attributes of the selected unit
        private TenetType tenetType;
        private List<AbilityLoadoutDialogue.AbilityInfo> currentAbilityInfos;
        
        // Holds the info for the new abilities
        private List<AbilityLoadoutDialogue.AbilityInfo> newAbilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();
        
        // References to the new ability buttons scripts
        [SerializeField] private List<AbilityButton> abilityButtons = new List<AbilityButton>();

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
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
        
        #region Drawing
        
        internal void Redraw(TenetType newTenetType, List<AbilityLoadoutDialogue.AbilityInfo> oldAbilityInfos)
        {
            tenetType = newTenetType;
            currentAbilityInfos = oldAbilityInfos;

            newAbilityInfos = GetAbilities(3, tenetType);
            
            // Assign ability images
            for (int i = 0; i < newAbilityInfos.Count; ++i)
            {
                abilityButtons[i].Redraw(
                    newAbilityInfos[i].Render,
                    newAbilityInfos[i].Ability.name,
                    newAbilityInfos[i].Ability.Description,
                    false);
            }
        }
        
        #endregion

        #region Utility Functions

        public void AddSelectedAbility(IUnit unit)
        {
            foreach (var abilityInfo in newAbilityInfos)
            {
                if (abilityInfo.Ability.name.Equals(currentSelectedAbility.AbilityName))
                {
                    unit.Abilities.Add(abilityInfo.Ability);
                    break;
                }
            }
        }
        
        private List<AbilityLoadoutDialogue.AbilityInfo> GetAbilities(int numberOfAbilities, TenetType tenetType)
        {
            List<AbilityLoadoutDialogue.AbilityInfo> abilityInfos = new List<AbilityLoadoutDialogue.AbilityInfo>();

            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            AbilityPool poolToUse = playerManager.AbilityPickupPool;

            if (poolToUse == null)
                poolToUse = abilityPool;

            // List<Ability> selectedAbilities = RandomiseAbilityOrder(poolToUse.PickAbilitiesByTenet(tenetType).ToList());
            List<Ability> selectedAbilities = poolToUse.PickAbilitiesByTenet(tenetType).ToList();

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
