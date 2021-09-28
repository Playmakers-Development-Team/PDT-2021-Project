using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.CombatEndUI.AbilityLoadout.PanelScripts;
using UI.Commands;
using UI.Core;
using Units.Players;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class AbilityUpgradeDialogue : Dialogue
    {
        internal readonly Event showUnitSelectPanel = new Event();
        internal readonly Event<LoadoutUnitInfo> showUpgradeSelectPanel = new Event<LoadoutUnitInfo>();
        
        internal readonly Event<LoadoutUnitInfo> unitSpawned = new Event<LoadoutUnitInfo>();
        internal readonly Event abilityUpgradeConfirm = new Event();
        internal readonly Event<AbilityButton> drawCurrentAbilityDetails = new Event<AbilityButton>();
        internal readonly Event<AbilityButton> drawUpgradedAbilityDetails = new Event<AbilityButton>();
        internal readonly Event clearCurrentAbilityDetails = new Event();
        internal readonly Event clearUpgradedAbilityDetails = new Event();
        internal readonly Event<AbilitySelectedCommand> abilityButtonPress = new Event<AbilitySelectedCommand>();
        
        private CommandManager commandManager;
        private UIManager uiManager;

        [SerializeField] private Canvas unitSelectCanvas;
        [SerializeField] private Canvas upgradeSelectCanvas;
        [SerializeField] private AbilityDetailsPanel currentAbilityDetailsPanel;
        [SerializeField] private AbilityDetailsPanel upgradedAbilityDetailsPanel;
        [SerializeField] protected UnitSelectCanvasScript unitSelectCanvasScript;
        [SerializeField] protected AbilitySelectCanvasScript upgradeSelectCanvasScript;
        
        private readonly List<LoadoutUnitInfo> units = new List<LoadoutUnitInfo>();
        public List<Sprite> abilityImages = new List<Sprite>();

        #region Monobehaviour Events

        protected override void OnDialogueAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            uiManager = ManagerLocator.Get<UIManager>();
            
            // Add to UI manager and show starting panel
            uiManager.Add(this);

            // Hide Panels
            unitSelectCanvas.enabled = false;
            upgradeSelectCanvas.enabled = false;

            // Listen to Events
            showUnitSelectPanel.AddListener(() =>
            {
                OnUnitSelectPanel();
            });

            showUpgradeSelectPanel.AddListener(unitInfo =>
            {
                OnUpgradeSelectPanel(unitInfo);
            });

            unitSpawned.AddListener(info =>
            {
                if (info.Unit is PlayerUnit)
                    units.Add(info);
            });
            
            abilityButtonPress.AddListener(AbilitySelectedCommand =>
            {
                if (AbilitySelectedCommand.IsNewAbility)
                {
                    upgradeSelectCanvasScript.OnAbilityButtonPress(AbilitySelectedCommand.
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
                
                // Assumption that only 1 unit is in unitCards during ability swap
                upgradeSelectCanvasScript.AddSelectedAbility(unitSelectCanvasScript.unitCards[0].loadoutUnitInfo.Unit);
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
        
        #region Panel Switching

        private void OnUnitSelectPanel()
        {
            unitSelectCanvas.enabled = true;
            upgradeSelectCanvas.enabled = false;
            
            unitSelectCanvasScript.Redraw(units);
        }
        
        private void OnUpgradeSelectPanel(LoadoutUnitInfo unitInfo)
        {
            upgradeSelectCanvas.enabled = true;
            
            // Clear the units so only the selected unit is shown
            units.Clear();
            units.Add(unitInfo);
            
            // Redraw the 1 ability, unit and new unit abilities
            unitSelectCanvasScript.Redraw(units);
            upgradeSelectCanvasScript.Redraw(unitInfo.Unit.Tenet, unitInfo.AbilityInfo);

            unitSelectCanvasScript.EnableAbilityButtons(unitInfo);
        }
        
        private void OnAbilitySelect(AbilitySelectedCommand cmd)
        {
            abilityButtonPress.Invoke(cmd);
        }

        #endregion

        #region Dialogue
        
        protected override void OnClose() {}

        protected override void OnPromote()
        {
            canvasGroup.interactable = true;
        }

        protected override void OnDemote()
        {
            canvasGroup.interactable = false;
        }
        
        #endregion
        
        #region Querying
        
        // TODO: Move into it's own thing later on
        internal LoadoutAbilityInfo GetInfo(Ability ability)
        {
            LoadoutAbilityInfo abilityInfo = new LoadoutAbilityInfo();
            abilityInfo.Ability = ability;
            
            switch (ability.RepresentedTenet)
            {
                case TenetType.Apathy:
                    abilityInfo.Render = abilityImages[0];
                    break;
                case TenetType.Humility:
                    abilityInfo.Render = abilityImages[1];
                    break;
                case TenetType.Joy:
                    abilityInfo.Render = abilityImages[2];
                    break;
                case TenetType.Passion:
                    abilityInfo.Render = abilityImages[3];
                    break;
                case TenetType.Pride:
                    abilityInfo.Render = abilityImages[4];
                    break;
                case TenetType.Sorrow:
                    abilityInfo.Render = abilityImages[5];
                    break;
                default:
                    throw new Exception($"Could not get {nameof(LoadoutAbilityInfo)} for {ability}.");
            }
            
            return abilityInfo;
        }

        #endregion
    }
}
