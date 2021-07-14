using System;
using System.Collections.Generic;
using Abilities;
using Abilities.Commands;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
using UnityEngine;

namespace UI
{
    public class GameDialogue : Dialogue
    {
        internal readonly Event<UnitInfo> unitSpawned = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitKilled = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitSelected = new Event<UnitInfo>();
        internal readonly Event unitDeselected = new Event();
        
        internal readonly Event<StatDifference> unitDamaged = new Event<StatDifference>();
        
        internal readonly Event<Ability> abilitySelected = new Event<Ability>();
        internal readonly Event abilityDeselected = new Event();
        internal readonly Event<Vector2> abilityRotated = new Event<Vector2>();
        internal readonly Event abilityConfirmed = new Event();
        
        internal readonly Event<TurnInfo> turnStarted = new Event<TurnInfo>();


        private CommandManager commandManager;
        private TurnManager turnManager;
        
        private readonly List<UnitInfo> units = new List<UnitInfo>();


        internal UnitInfo SelectedUnit { get; private set; }
        
        internal Ability SelectedAbility { get; private set; }
        
        internal Vector2 AbilityDirection { get; private set; }
        
        
        #region MonoBehaviour Events

        internal override void OnAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();

            // Listen to Events
            unitSpawned.AddListener(info =>
            {
                units.Add(info);
            });
            
            unitKilled.AddListener(info =>
            {
                units.Remove(info);
            });
            
            unitSelected.AddListener(unit =>
            {
                bool changed = SelectedUnit != unit;
                
                SelectedUnit = unit;
            
                if (changed)
                    abilityDeselected.Invoke(); 
            });
            
            unitDeselected.AddListener(() =>
            {
                SelectedUnit = null;
                abilityDeselected.Invoke();
            });
            
            abilitySelected.AddListener(ability =>
            {
                SelectedAbility = ability;
            });
            
            abilityDeselected.AddListener(() =>
            {
                SelectedAbility = null;
            });
            
            abilityRotated.AddListener(direction =>
            {
                AbilityDirection = direction;
            });
            
            abilityConfirmed.AddListener(() =>
            {
                commandManager.ExecuteCommand(new AbilityCommand(SelectedUnit.Unit, AbilityDirection, SelectedAbility));
            });
        }

        private void OnEnable()
        {
            // Subscribe to Commands
            commandManager.ListenCommand((Action<StartTurnCommand>) OnStartTurn);
            commandManager.ListenCommand((Action<TakeTotalDamageCommand>) OnUnitDamaged);
            commandManager.ListenCommand((Action<KilledUnitCommand>) OnUnitKilled);
        }

        private void OnDisable()
        {
            // Unsubscribe from Commands
            commandManager.UnlistenCommand((Action<StartTurnCommand>) OnStartTurn);
            commandManager.UnlistenCommand((Action<TakeTotalDamageCommand>) OnUnitDamaged);
            commandManager.UnlistenCommand((Action<KilledUnitCommand>) OnUnitKilled);
        }
        
        #endregion


        #region Command Listeners

        private void OnStartTurn(StartTurnCommand cmd)
        {
            UnitInfo info = GetInfo(cmd.Unit);

            if (info == null)
                throw new Exception("ActingUnit was not in GameDialogue.units.");
            
            turnStarted.Invoke(new TurnInfo(info));
        }

        private void OnUnitDamaged(TakeTotalDamageCommand cmd)
        {
            unitDamaged.Invoke(new StatDifference(cmd));
        }

        private void OnUnitKilled(KilledUnitCommand cmd)
        {
            UnitInfo info = GetInfo(cmd.Unit);

            if (info == null)
                throw new Exception("Killed Unit was not in GameDialogue.units.");

            unitKilled.Invoke(info);
        }
        
        #endregion


        #region Dialogue Event Functions

        protected override void OnHide() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
        
        #endregion
        
        
        #region Utility Functions

        internal UnitInfo GetInfo(IUnit unit) => units.Find(u => u.Unit == unit);
        
        #endregion
        
        
        #region Structs

        [Serializable]
        internal class UnitInfo
        {
            [SerializeField] private Sprite render;
            [SerializeField] private Color color;

            
            internal Sprite Render => render;
            internal Color Color => color;
            
            internal IUnit Unit { get; private set; }


            internal void SetUnit(IUnit newUnit) => Unit = newUnit;
        }

        [Serializable]
        internal class TurnInfo
        {
            internal UnitInfo CurrentUnit { get; }


            public TurnInfo(UnitInfo currentUnit)
            {
                CurrentUnit = currentUnit;
            }
        }
        
        #endregion
    }
}
