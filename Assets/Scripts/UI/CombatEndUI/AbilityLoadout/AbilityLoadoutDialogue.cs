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

namespace UI.CombatEndUI.AbilityLoadout
{
    public partial class AbilityLoadoutDialogue : Dialogue
    {
        internal readonly Event showUnitSelectPanel = new Event();
        internal readonly Event<LoadoutUnitInfo> showAbilitySelectPanel = new Event<LoadoutUnitInfo>();
        
        internal readonly Event<LoadoutUnitInfo> unitSpawned = new Event<LoadoutUnitInfo>();
        internal readonly Event abilitySwapConfirm = new Event();
        internal readonly Event<AbilityButton> drawOldAbilityDetails = new Event<AbilityButton>();
        internal readonly Event<AbilityButton> drawNewAbilityDetails = new Event<AbilityButton>();
        internal readonly Event clearOldAbilityDetails = new Event();
        internal readonly Event clearNewAbilityDetails = new Event();
        internal readonly Event<AbilitySelectedCommand> abilityButtonPress = new Event<AbilitySelectedCommand>();
        
        private CommandManager commandManager;
        private UIManager uiManager;

        [SerializeField] private Canvas unitSelectCanvas;
        [SerializeField] private Canvas abilitySelectCanvas;
        [SerializeField] private AbilityDetailsPanel oldAbilityDetailsPanel;
        [SerializeField] private AbilityDetailsPanel newAbilityDetailsPanel;
        [SerializeField] protected UnitSelectCanvasScript unitSelectCanvasScript;
        [SerializeField] protected AbilitySelectCanvasScript abilitySelectCanvasScript;
        
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
            abilitySelectCanvas.enabled = false;

            // Listen to Events
            showUnitSelectPanel.AddListener(() =>
            {
                OnUnitSelectPanel();
            });

            showAbilitySelectPanel.AddListener(unitInfo =>
            {
                OnAbilitySelectPanel(unitInfo);
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
                    abilitySelectCanvasScript.OnAbilityButtonPress(AbilitySelectedCommand.
                        AbilityButton);
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
                
                // Assumption that only 1 unit is in unitCards during ability swap
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
        
        #region Panel Switching

        private void OnUnitSelectPanel()
        {
            unitSelectCanvas.enabled = true;
            abilitySelectCanvas.enabled = false;
            
            unitSelectCanvasScript.Redraw(units);
        }
        
        private void OnAbilitySelectPanel(LoadoutUnitInfo loadoutUnitInfo)
        {
            abilitySelectCanvas.enabled = true;
            
            // Clear the units so only the selected unit is shown
            units.Clear();
            units.Add(loadoutUnitInfo);
            
            // Redraw the 1 ability, unit and new unit abilities
            unitSelectCanvasScript.Redraw(units);
            abilitySelectCanvasScript.Redraw(loadoutUnitInfo.Unit.Tenet, loadoutUnitInfo.AbilityInfo);

            unitSelectCanvasScript.EnableAbilityButtons(loadoutUnitInfo);
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
            LoadoutAbilityInfo loadoutAbilityInfo = new LoadoutAbilityInfo();
            loadoutAbilityInfo.Ability = ability;
            
            switch (ability.RepresentedTenet)
            {
                case TenetType.Apathy:
                    loadoutAbilityInfo.Render = abilityImages[0];
                    break;
                case TenetType.Humility:
                    loadoutAbilityInfo.Render = abilityImages[1];
                    break;
                case TenetType.Joy:
                    loadoutAbilityInfo.Render = abilityImages[2];
                    break;
                case TenetType.Passion:
                    loadoutAbilityInfo.Render = abilityImages[3];
                    break;
                case TenetType.Pride:
                    loadoutAbilityInfo.Render = abilityImages[4];
                    break;
                case TenetType.Sorrow:
                    loadoutAbilityInfo.Render = abilityImages[5];
                    break;
                default:
                    throw new Exception($"Could not get {nameof(LoadoutAbilityInfo)} for {ability}.");
            }
            
            return loadoutAbilityInfo;
        }

        #endregion
    }
}
