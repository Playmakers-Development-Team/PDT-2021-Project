using System;
using Commands;
using Managers;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.Commands;
using UI.Core;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class AbilityUpgradeDialogue : AbilityRewardDialogue
    {
        internal readonly Event abilityUpgradeConfirm = new Event();
        internal readonly Event<AbilityButton> drawCurrentAbilityDetails = new Event<AbilityButton>();
        internal readonly Event<AbilityButton> drawUpgradedAbilityDetails = new Event<AbilityButton>();
        internal readonly Event clearCurrentAbilityDetails = new Event();
        internal readonly Event clearUpgradedAbilityDetails = new Event();

        private CommandManager commandManager;
        private UIManager uiManager;
        
        [SerializeField] private AbilityDetailsPanel currentAbilityDetailsPanel;
        [SerializeField] private AbilityDetailsPanel upgradedAbilityDetailsPanel;

        #region Monobehaviour Events

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();
            
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            uiManager = ManagerLocator.Get<UIManager>();
            
            // Add to UI manager and show starting panel
            uiManager.Add(this);

            // Hide Panels
            unitSelectCanvas.enabled = false;
            abilitySelectCanvas.enabled = false;

            // Listen to Events
            abilityButtonPress.AddListener(AbilitySelectedCommand =>
            {
                if (AbilitySelectedCommand.IsNewAbility)
                {
                    abilitySelectCanvasScript.OnAbilityButtonPress(AbilitySelectedCommand.
                        AbilityButton);
                }
                else
                {
                    unitSelectCanvasScript.OnAbilityButtonPress(
                        AbilitySelectedCommand.AbilityButton);
                    
                }
            });

            drawCurrentAbilityDetails.AddListener(AbilityButton => 
            {
                currentAbilityDetailsPanel.Redraw(AbilityButton);
            });
            
            drawUpgradedAbilityDetails.AddListener(AbilityButton => 
            {
                upgradedAbilityDetailsPanel.Redraw(AbilityButton);
            });
            
            clearCurrentAbilityDetails.AddListener(() => 
            {
                currentAbilityDetailsPanel.ClearValues();
            });
            
            clearUpgradedAbilityDetails.AddListener(() => 
            {
                upgradedAbilityDetailsPanel.ClearValues();
            });
            
            abilityUpgradeConfirm.AddListener(() =>
            {
                // Skip ability upgrade if there is an empty old ability or upgraded ability
                if (currentAbilityDetailsPanel.abilityName.text.Equals("") || 
                    upgradedAbilityDetailsPanel.abilityName.text.Equals(""))
                    return;
                
                unitSelectCanvasScript.RemoveSelectedAbility();
                
                // Assumption that only 1 unit is in unitCards during ability upgrade
                abilitySelectCanvasScript.AddSelectedAbility(unitSelectCanvasScript.unitCards[0].loadoutUnitInfo.Unit);
            });
            
            // Execute ready command to inform UnitLoadoutUIWrapper
            commandManager.ExecuteCommand(new AbilityRewardDialogueReadyCommand());
            
            // Show the unit select panel after the AbilityLoadoutReadyCommand
            // so that unit data from UnitLoadoutUIWrapper can be retrieved
            showUnitSelectPanel.Invoke();
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
        
        protected override void OnAbilitySelectPanel(LoadoutUnitInfo loadoutUnitInfo)
        {
            abilitySelectCanvas.enabled = true;
            
            // Clear the units so only the selected unit is shown
            units.Clear();
            units.Add(loadoutUnitInfo);
            
            // Redraw the 1 unit and unit abilities
            unitSelectCanvasScript.Redraw(units);
            
            // Redraw ability upgrade options
            abilitySelectCanvasScript.RedrawForUpgrade(loadoutUnitInfo.AbilityInfo);
        }
    }
}
