using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Game.Commands;
using Managers;
using Turn.Commands;
using UI.AbilityLoadout.Unit;
using UI.Core;
using Units;
using Units.Players;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.AbilityLoadout
{
    public class AbilityLoadoutDialogue : Dialogue
    {
        internal readonly Event<AbilityLoadoutPanelType> panelSwap = new Event<AbilityLoadoutPanelType>();
        internal readonly Event<UnitInfo> unitSpawned = new Event<UnitInfo>();
        internal readonly Event noEnemiesRemaining = new Event();
        
        private CommandManager commandManager;

        [SerializeField] private Canvas unitSelectPanel;
        [SerializeField] private Canvas abilitySelectPanel;
        [SerializeField] protected AbilityLoadoutUnitList abilityLoadoutUnitList;
        
        private readonly List<UnitInfo> units = new List<UnitInfo>();

        #region Monobehaviour Events

        internal override void OnAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            
            // Hide Panels
            unitSelectPanel.enabled = false;
            abilitySelectPanel.enabled = false;

            // Listen to Events
            panelSwap.AddListener(currentPanel =>
            {
                OnSwitchPanel(currentPanel);
            });
            
            unitSpawned.AddListener(info =>
            {
                if (info.Unit is PlayerUnit)
                    units.Add(info);
            });

            noEnemiesRemaining.AddListener(() =>
            {
                // TODO: Change to appear after the player selects this option
                panelSwap.Invoke(AbilityLoadoutPanelType.UnitSelect);
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

        private void OnSwitchPanel(AbilityLoadoutPanelType currentPanel)
        {
            if (currentPanel == AbilityLoadoutPanelType.UnitSelect)
                OnUnitSelectPanel();
            else
                OnAbilitySelectPanel();
        }
        
        private void OnUnitSelectPanel()
        {
            unitSelectPanel.enabled = true;
            abilitySelectPanel.enabled = false;
            
            abilityLoadoutUnitList.Redraw(units);
        }
        
        private void OnAbilitySelectPanel()
        {
            unitSelectPanel.enabled = false;
            abilitySelectPanel.enabled = true;
        }

        #endregion
        
        #region Command Listeners

        private void OnNoEnemiesRemaining(NoRemainingEnemyUnitsCommand cmd)
        {
            noEnemiesRemaining.Invoke();
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

        internal UnitInfo GetInfo(IUnit unit)
        {
            if (units.Count == 0)
            {
                throw new Exception($"Could not get {nameof(UnitInfo)} for {unit}. " +
                                    $"{nameof(AbilityLoadoutDialogue)}.{nameof(units)} is empty.");
            }

            var unitInfo = units.Find(u => u.Unit == unit);
                
            if (unitInfo == null)
            {
                throw new Exception($"Could not get {nameof(UnitInfo)} for {unit}. " +
                                    $"{unit} is not in {nameof(AbilityLoadoutDialogue)}.{nameof(units)}.");
            }

            return unitInfo;
        }

        #endregion
        
        #region Structs
        
        [Serializable]
        public class UnitInfo
        {
            [SerializeField] private CropInfo profileCropInfo;
            
            internal CropInfo ProfileCropInfo => profileCropInfo;

            public IUnit Unit { get; private set; }
            
            internal void SetUnit(IUnit newUnit) => Unit = newUnit;
        }
        
        [Serializable]
        public class AbilityInfo
        {
            [SerializeField] private Sprite render;
            
            internal Sprite Render => render;

            public Ability Ability { get; private set; }
            
            internal void SetAbility(Ability newUnit) => Ability = newUnit;
        }

        
        #endregion
    }
}
