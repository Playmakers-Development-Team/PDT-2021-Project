using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using Turn.Commands;
using UI.AbilityLoadout.Abilities;
using UI.Commands;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.AbilityLoadout.Panel_Scripts
{
    public class AbilityLoadoutSelectionPanel : DialogueComponent<AbilityLoadoutDialogue>
    {
        // Placeholder selection sprite and sprite offset
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        // Used to identify the currently selected new ability
        internal readonly Event<AbilityButton> abilityButtonPress = new Event<AbilityButton>();
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

        private CommandManager commandManager;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion

        #region Monobehaviour Functions

        protected override void OnComponentAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            
            // Listeners
            abilityButtonPress.AddListener(abilityButton =>
            {
                OnAbilityButtonPress(abilityButton);
            });

            foreach (var abilityButton in abilityButtons)
            {
                abilityButton.AbilityButtonAwake();
            }
        }
        
        private void OnEnable()
        {
            commandManager.ListenCommand((Action<AbilitySelectedCommand>) OnAbilitySelect);
        }
        
        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<AbilitySelectedCommand>) OnAbilitySelect);
        }

        #endregion
        
        #region Listeners

        private void OnAbilitySelect(AbilitySelectedCommand cmd)
        {
            abilityButtonPress.Invoke(cmd.AbilityButton);
        }

        private void OnAbilityButtonPress(AbilityButton abilityButton)
        {
            Debug.LogWarning("TEST");
            if (currentSelectedAbility == abilityButton)
            {
                // Make no ability selected
                currentSelectedAbility = null;
                
                // Turn Off Visual Placeholder
                selectedAbilityImage.enabled = false;
            }
            else
            {
                // Deselect the old ability
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
                    newAbilityInfos[i].Ability.Description);
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
