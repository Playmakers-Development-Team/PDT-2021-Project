using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using UI.CombatEndUI.AbilityUpgrading;
using UI.Commands;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.CombatEndUI.PanelScripts
{
    public class AbilitySelectCanvasScript : DialogueComponent<AbilityRewardDialogue>
    {
        [Header("Canvas")]
        [SerializeField] protected Canvas abilitySelectCanvas;
        [SerializeField] protected Canvas finalAbilitiesCanvas;
        
        // New ability buttons and script references
        [SerializeField] private GameObject newAbilityPrefab;
        [SerializeField] private int newAbilityCount = 3;
        [SerializeField] private int upgradeAbilityCap = int.MaxValue;
        [SerializeField] private ScrollRect abilityScrollView;
        private List<AbilityButton> abilityButtons = new List<AbilityButton>();
        
        // Placeholder selection sprite and sprite offset
        [SerializeField] private Image selectedAbilityImage;
        [SerializeField] private Vector3 selectedOffset;
        
        // Used to identify the currently selected new ability
        private AbilityButton currentSelectedAbility;
        
        // Holds data for all abilities in the game, used to obtain new abilities
        [SerializeField] private AbilityPool abilityPool;
        
        // Attributes of the selected unit
        private TenetType tenetType;
        private List<LoadoutAbilityInfo> currentAbilityInfos;
        
        // Holds the info for the new abilities
        private List<LoadoutAbilityInfo> newAbilityInfos = new List<LoadoutAbilityInfo>();

        [Header("Animators")]
        [SerializeField] private Animator panelSlideAnim;
        [SerializeField] private Animator panelFadeAnim;
        [SerializeField] private Animator buttonFadeAnim;
        [SerializeField] private Animator returnButtonFadeAnim;

        private PlayerManager playerManager;
        
        private AbilityPool PreferredAbilityPool => playerManager.AbilityPickupPool != null
            ? playerManager.AbilityPickupPool
            : abilityPool;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            abilityScrollView.onValueChanged.AddListener(UpdateAbilityScroll);
            playerManager = ManagerLocator.Get<PlayerManager>();
        }
        
        private void UpdateAbilityScroll(Vector2 arg0)
        {
            DeselectAbility();
        }
        
        #endregion
        
        #region Listeners

        public void OnAbilityButtonPress(AbilityButton abilityButton)
        {
            if (currentSelectedAbility == abilityButton)
            {
                currentSelectedAbility.Deselect();
                
                //dialogue.GetNonUpgradedAbility()
                
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
        
        internal void RedrawForLoadout(TenetType newTenetType, List<LoadoutAbilityInfo> oldAbilityInfos)
        {
            DestroyAbilityList();
            
            tenetType = newTenetType;
            currentAbilityInfos = oldAbilityInfos;

            newAbilityInfos = GetAbilities(newAbilityCount, tenetType);
            
            // Instantiate new AbilityButtons
            foreach (LoadoutAbilityInfo abilityInfo in newAbilityInfos)
            {
                AbilityButton newAbilityButton = Instantiate(newAbilityPrefab, abilityScrollView.content).GetComponent<AbilityButton>();
                newAbilityButton.Redraw(
                    abilityInfo.Render,
                    abilityInfo.Ability,
                    false);

                abilityButtons.Add(newAbilityButton);
            }
            
            panelSlideAnim.SetTrigger("Play");
            Invoke("FadeInElements", 0.5f);
        }
        
        internal void RedrawForUpgrade(List<LoadoutAbilityInfo> oldAbilityInfos)
        {
            DestroyAbilityList();
            
            currentAbilityInfos = oldAbilityInfos;

            newAbilityInfos = GetUpgrades(oldAbilityInfos);
            
            // Instantiate new AbilityButtons
            foreach (LoadoutAbilityInfo abilityInfo in newAbilityInfos)
            {
                AbilityButton newAbilityButton = Instantiate(newAbilityPrefab, abilityScrollView.content).GetComponent<AbilityButton>();
                newAbilityButton.Redraw(
                    abilityInfo.Render,
                    abilityInfo.Ability,
                    false);

                abilityButtons.Add(newAbilityButton);
            }
            
            panelSlideAnim.SetTrigger("Play");
            Invoke("FadeInElements", 1.5f);
        }

        private void DestroyAbilityList()
        {
            for (int i = abilityButtons.Count - 1; i >= 0; i--)
            {
                Destroy(abilityButtons[i].gameObject);
                abilityButtons.RemoveAt(i);
            }
        }
        
        #endregion

        #region Utility Functions

        public void UpdateCurrentAbilities(List<LoadoutAbilityInfo> abilityInfos)
        {
            currentAbilityInfos = abilityInfos;
        }
        
        public void ShowAbilitySelectCanvas()
        {
            abilitySelectCanvas.enabled = true;
            finalAbilitiesCanvas.enabled = true;
        }

        public void HideAbilitySelectCanvas()
        {
            DeselectAbility();
            
            abilitySelectCanvas.enabled = false;
            finalAbilitiesCanvas.enabled = false;
        }

        private void DeselectAbility()
        {
            if(currentSelectedAbility != null)
                currentSelectedAbility.Deselect();
                
            // Make no ability selected
            currentSelectedAbility = null;
                
            // Turn Off Visual Placeholder
            selectedAbilityImage.enabled = false;
        }
        
        private void FadeInElements()
        {
            panelFadeAnim.SetTrigger("Play");
            buttonFadeAnim.SetTrigger("Play");

            // The return button should only be available if we're ability upgrading
            if (dialogue.GetType() == typeof(AbilityUpgradeDialogue))
                returnButtonFadeAnim.SetTrigger("Play");
        }
        
        public void AddSelectedAbility()
        {
            // Don't add a new ability if the user has more than 4 abilities
            if (currentAbilityInfos.Count >= 4)
                return;
            
            foreach (var abilityInfo in newAbilityInfos)
            {
                if (abilityInfo.Ability.name.Equals(currentSelectedAbility.AbilityName))
                {
                    dialogue.activeUnitCard.loadoutUnitInfo.Unit.Abilities.Add(abilityInfo.Ability);
                    break;
                }
            }
        }
        
        private List<LoadoutAbilityInfo> GetAbilities(int numberOfAbilities, TenetType tenetType)
        {
            List<LoadoutAbilityInfo> abilityInfos = new List<LoadoutAbilityInfo>();
            List<Ability> selectedAbilities = PreferredAbilityPool.PickAbilitiesByTenet(tenetType).ToList();

            for (int i = 0; i < selectedAbilities.Count; ++i)
            {
                LoadoutAbilityInfo newLoadoutAbility =
                    dialogue.GetInfo(selectedAbilities[i]);
                
                // Skip the current iteration if the character already owns the ability OR
                // Skip the current iteration if the ability is an upgrade
                if(currentAbilityInfos.Contains(newLoadoutAbility) || newLoadoutAbility.Ability.name.Contains("+"))
                    continue;
                
                abilityInfos.Add(newLoadoutAbility);
                
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
                    LoadoutAbilityInfo newLoadoutAbility =
                        dialogue.GetInfo(selectedAbilities[i]);

                    abilityInfos.Add(newLoadoutAbility);
                
                    numberOfAbilities--;
                    if (numberOfAbilities <= 0)
                        break;
                }
            }
            
            return abilityInfos;
        }
        
        private List<LoadoutAbilityInfo> GetUpgrades(List<LoadoutAbilityInfo> oldAbilityInfos)
        {
            List<LoadoutAbilityInfo> upgradedAbilityInfos = new List<LoadoutAbilityInfo>();

            for (int i = 0; i < oldAbilityInfos.Count; ++i)
            {
                String upgradedAbilityName = oldAbilityInfos[i].Ability.name + "+";
                
                Ability upgradedAbility = PreferredAbilityPool.PickAbilitiesByName(upgradedAbilityName);
                
                if(upgradedAbility != null)
                    upgradedAbilityInfos.Add(dialogue.GetInfo(upgradedAbility));

                if (upgradedAbilityInfos.Count == upgradeAbilityCap)
                    break;
            }

            if (upgradedAbilityInfos.Count == 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(new NoUpgradesCommand());
            else
                ManagerLocator.Get<CommandManager>().ExecuteCommand(new UpgradesAvailableCommand());

            return upgradedAbilityInfos;
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
