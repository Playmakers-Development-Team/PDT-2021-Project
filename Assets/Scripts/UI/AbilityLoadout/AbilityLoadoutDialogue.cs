using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using Turn.Commands;
using UI.AbilityLoadout.Abilities;
using UI.AbilityLoadout.Panel_Scripts;
using UI.Core;
using Units;
using Units.Players;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.AbilityLoadout
{
    public class AbilityLoadoutDialogue : Dialogue
    {
        internal readonly Event showUnitSelectPanel = new Event();
        internal readonly Event<UnitInfo> showAbilitySelectPanel = new Event<UnitInfo>();
        
        internal readonly Event<UnitInfo> unitSpawned = new Event<UnitInfo>();
        internal readonly Event noEnemiesRemaining = new Event();
        internal readonly Event abilitySwap = new Event();
        
        private CommandManager commandManager;
        private UIManager uiManager;

        [SerializeField] private Canvas unitSelectPanel;
        [SerializeField] private Canvas abilitySelectPanel;
        [SerializeField] protected AbilityLoadoutUnitPanel abilityLoadoutUnitPanel;
        [SerializeField] protected AbilityLoadoutSelectionPanel abilityLoadoutSelectionPanel;
        
        private readonly List<UnitInfo> units = new List<UnitInfo>();
        public List<Sprite> abilityImages = new List<Sprite>();

        #region Monobehaviour Events

        internal override void OnAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            uiManager = ManagerLocator.Get<UIManager>();
            
            // Hide Panels
            unitSelectPanel.enabled = false;
            abilitySelectPanel.enabled = false;

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

            noEnemiesRemaining.AddListener(() =>
            {
                uiManager.Add(this);
                
                // TODO: Change to appear after the player selects this option
                showUnitSelectPanel.Invoke();
            });
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoEnemiesRemaining);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoEnemiesRemaining);
        }

        #endregion
        
        #region Panel Switching

        private void OnUnitSelectPanel()
        {
            unitSelectPanel.enabled = true;
            abilitySelectPanel.enabled = false;
            
            abilityLoadoutUnitPanel.Redraw(units);
        }
        
        private void OnAbilitySelectPanel(UnitInfo unitInfo)
        {
            abilitySelectPanel.enabled = true;
            
            // Clear the units so only the selected unit is shown
            units.Clear();
            units.Add(unitInfo);
            
            abilityLoadoutUnitPanel.Redraw(units);
            abilityLoadoutSelectionPanel.Redraw(unitInfo.Unit.Tenet);
        }

        #endregion
        
        #region Command Listeners

        private void OnNoEnemiesRemaining(NoRemainingEnemyUnitsCommand cmd)
        {
            noEnemiesRemaining.Invoke();
        }
        
        //TODO: Add in ability swap command
        private void OnAbilitySwap()
        {
            abilitySwap.Invoke();
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
        internal AbilityInfo GetInfo(Ability ability)
        {
            AbilityInfo abilityInfo = new AbilityInfo();
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
                    throw new Exception($"Could not get {nameof(AbilityInfo)} for {ability}.");
            }
            
            return abilityInfo;
        }

        #endregion
        
        #region Structs
        
        [Serializable]
        public class UnitInfo
        {
            [SerializeField] private CropInfo profileCropInfo;
            [SerializeField] private List<AbilityInfo> abilityInfo;
            
            internal CropInfo ProfileCropInfo => profileCropInfo;

            public IUnit Unit { get; private set; }
            public List<AbilityInfo> AbilityInfo { get; private set; }
            
            internal void SetUnit(IUnit newUnit) => Unit = newUnit;
            internal void SetAbilityInfo(List<AbilityInfo> newAbilityInfo) => AbilityInfo = newAbilityInfo;
        }
        
        [Serializable]
        public class AbilityInfo
        {
            [SerializeField] private Sprite render;
            
            internal Sprite Render
            {
                get => render;
                set => render = value;
            }

            public Ability Ability { get; internal set; }
            
            internal void SetAbility(Ability newUnit) => Ability = newUnit;
        }

        
        #endregion
    }
}
