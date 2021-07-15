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

namespace UI.Game
{
    public class GameDialogue : Dialogue
    {
        internal readonly Event<UnitInfo> unitSpawned = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitKilled = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitSelected = new Event<UnitInfo>();
        internal readonly Event unitDeselected = new Event();
        
        internal readonly Event<StatChangeInfo> unitDamaged = new Event<StatChangeInfo>();
        
        internal readonly Event<Ability> abilitySelected = new Event<Ability>();
        internal readonly Event<Ability> abilityDeselected = new Event<Ability>();
        internal readonly Event<Vector2> abilityRotated = new Event<Vector2>();
        internal readonly Event abilityConfirmed = new Event();
        
        internal readonly Event<TurnInfo> turnStarted = new Event<TurnInfo>();

        internal readonly Event<UnitInfo> delayConfirmed = new Event<UnitInfo>();
        internal readonly Event<MoveInfo> moveConfirmed = new Event<MoveInfo>();


        private CommandManager commandManager;
        private TurnManager turnManager;
        
        private readonly List<UnitInfo> units = new List<UnitInfo>();


        internal UnitInfo SelectedUnit { get; private set; }
        
        internal Ability SelectedAbility { get; private set; }
        
        internal Vector2 AbilityDirection { get; private set; }

        private bool IsAbilitySelected => SelectedAbility != null;
        
        
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
                if (SelectedUnit == info)
                    unitDeselected.Invoke();
                
                units.Remove(info);
            });
            
            unitSelected.AddListener(unit =>
            {
                bool changed = SelectedUnit != unit;
                
                if (changed)
                    unitDeselected.Invoke();
                
                SelectedUnit = unit;
            });
            
            unitDeselected.AddListener(() =>
            {
                SelectedUnit = null;
            });
            
            abilitySelected.AddListener(ability =>
            {
                if (SelectedAbility != null)
                    abilityDeselected.Invoke(SelectedAbility);
                
                SelectedAbility = ability;
            });
            
            abilityDeselected.AddListener(ability =>
            {
                SelectedAbility = null;
            });
            
            abilityRotated.AddListener(direction =>
            {
                AbilityDirection = direction;
            });
            
            abilityConfirmed.AddListener(() =>
            {
                if (turnManager.ActingPlayerUnit == null || !IsAbilitySelected)
                    return;

                commandManager.ExecuteCommand(new AbilityCommand(turnManager.ActingPlayerUnit, AbilityDirection, SelectedAbility));
            });
            
            turnStarted.AddListener(info =>
            {
                abilityDeselected.Invoke(SelectedAbility);
            });
            
            delayConfirmed.AddListener(info =>
            {
                commandManager.ExecuteCommand(new EndTurnCommand(info.Unit));
            });
            
            moveConfirmed.AddListener(info =>
            {
                commandManager.ExecuteCommand(new StartMoveCommand(info.UnitInfo.Unit, info.Destination));
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
            unitDamaged.Invoke(new StatChangeInfo(cmd));
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

        internal readonly struct TurnInfo
        {
            internal UnitInfo CurrentUnit { get; }


            public TurnInfo(UnitInfo currentUnit)
            {
                CurrentUnit = currentUnit;
            }
        }
        
        internal readonly struct StatChangeInfo
        {
            internal IUnit Unit { get; }
            internal int NewValue { get; }
            internal int OldValue { get; }
            internal int BaseValue { get; }
            internal int Difference { get; }

            internal StatChangeInfo(TakeTotalDamageCommand cmd)
            {
                // TODO: Update so that Difference is positive when stat going up, negative when going down..
                // TODO: Have this constructor simply call the other...
                Unit = cmd.Unit;
                NewValue = Unit.Health.HealthPoints.Value;
                OldValue = NewValue + cmd.Value;
                BaseValue = Unit.Health.HealthPoints.BaseValue;
                Difference = OldValue - NewValue;
            }

            internal StatChangeInfo(IUnit unit, int newValue, int oldValue, int baseValue)
            {
                Unit = unit;
                NewValue = newValue;
                OldValue = oldValue;
                BaseValue = baseValue;
                Difference = oldValue - newValue;
            }
        }

        internal readonly struct MoveInfo
        {
            internal Vector2Int Destination { get; }
            internal UnitInfo UnitInfo { get; }


            public MoveInfo(Vector2Int destination, UnitInfo unitInfo)
            {
                Destination = destination;
                UnitInfo = unitInfo;
            }
        }
        
        #endregion
    }
}
