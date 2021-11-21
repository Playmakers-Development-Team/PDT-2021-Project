using System;
using Commands;
using Managers;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.Commands;
using UI.Core;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.CombatEndUI.AbilityLoadout
{
    public class AbilityLoadoutDialogue : AbilityRewardDialogue
    {
        internal readonly Event abilitySwapConfirm = new Event();

        private CommandManager commandManager;
        private UIManager uiManager;
        
        [SerializeField] private AbilityDetailsPanel oldAbilityDetailsPanel;
        [SerializeField] private AbilityDetailsPanel newAbilityDetailsPanel;

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
            finalAbilitiesCanvas.enabled = false;

            // Listen to Events
            abilityButtonPress.AddListener(AbilitySelectedCommand =>
            {
                if (AbilitySelectedCommand.IsNewAbility)
                {
                    abilitySelectCanvasScript.OnAbilityButtonPress(
                        AbilitySelectedCommand.AbilityButton);
                }
                else
                {
                    unitSelectCanvasScript.OnAbilityButtonPress(
                        AbilitySelectedCommand.AbilityButton);
                }
            });

            drawOldAbilityDetails.AddListener(AbilityButton => 
            {
                oldAbilityDetailsPanel.Redraw(AbilityButton);
            });
            
            drawNewAbilityDetails.AddListener(AbilityButton => 
            {
                newAbilityDetailsPanel.Redraw(AbilityButton);
            });
            
            clearOldAbilityDetails.AddListener(() => 
            {
                oldAbilityDetailsPanel.ClearValues();
            });
            
            clearNewAbilityDetails.AddListener(() => 
            {
                newAbilityDetailsPanel.ClearValues();
            });
            
            abilitySwapConfirm.AddListener(() =>
            {
                // Skip ability swap if there is an empty old or new ability
                if (oldAbilityDetailsPanel.abilityName.text.Equals("") || 
                    newAbilityDetailsPanel.abilityName.text.Equals(""))
                    return;
                
                unitSelectCanvasScript.RemoveSelectedAbility();
                
                abilitySelectCanvasScript.AddSelectedAbility();
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
            base.OnAbilitySelectPanel(loadoutUnitInfo);
            
            // Redraw new abilities and enable current ability buttons
            abilitySelectCanvasScript.RedrawForLoadout(loadoutUnitInfo.Unit.Tenet, loadoutUnitInfo.AbilityInfo);
            unitSelectCanvasScript.EnableAbilityButtons();
        }
    }
}