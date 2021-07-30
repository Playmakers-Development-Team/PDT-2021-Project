using System;
using System.Collections.Generic;
using Abilities;
using Abilities.Commands;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UI.Core;
using Units;
using Units.Commands;
using Units.Stats;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.Game
{
    public class GameDialogue : Dialogue
    {
        internal readonly Event<UnitInfo> unitSpawned = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitKilled = new Event<UnitInfo>();
        internal readonly Event<UnitInfo> unitSelected = new Event<UnitInfo>();
        internal readonly Event unitDeselected = new Event();
        internal readonly Event<MoveInfo> startedMove = new Event<MoveInfo>();
        internal readonly Event<UnitInfo> endedMove = new Event<UnitInfo>();
        
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
            commandManager.ListenCommand((Action<StartMoveCommand>) OnStartMove);
            commandManager.ListenCommand((Action<EndMoveCommand>) OnEndMove);
            commandManager.ListenCommand((Action<StatChangedCommand>) OnUnitDamaged);
            commandManager.ListenCommand((Action<KilledUnitCommand>) OnUnitKilled);
        }

        private void OnDisable()
        {
            // Unsubscribe from Commands
            commandManager.UnlistenCommand((Action<StartTurnCommand>) OnStartTurn);
            commandManager.UnlistenCommand((Action<StartMoveCommand>) OnStartMove);
            commandManager.UnlistenCommand((Action<EndMoveCommand>) OnEndMove);
            commandManager.UnlistenCommand((Action<StatChangedCommand>) OnUnitDamaged);
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

        private void OnStartMove(StartMoveCommand cmd)
        {
            startedMove.Invoke(new MoveInfo(cmd.TargetCoords, GetInfo(cmd.Unit)));
        }

        private void OnEndMove(EndMoveCommand cmd)
        {
            endedMove.Invoke(GetInfo(cmd.Unit));
        }

        private void OnUnitDamaged(StatChangedCommand cmd)
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

        internal UnitInfo GetInfo(IUnit unit) => units.Find(u => u.Unit == unit);
        
        #endregion
        
        
        #region Structs

        // TODO: Turn this into a struct, null comparison can be made on UnitInfo.Unit...
        [Serializable]
        public class UnitInfo
        {
            [SerializeField] private Sprite render;
            [SerializeField] private Color color;

            
            internal Sprite Render => render;
            internal Color Color => color;
            
            public IUnit Unit { get; private set; }


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
            // TODO: Same as OldValue, may be being used incorrectly.
            internal int BaseValue { get; }
            internal int Difference { get; }
            internal StatTypes StatType { get; }

            internal StatChangeInfo(StatChangedCommand cmd)
            {
                StatType = cmd.StatType;
                Unit = cmd.Unit;
                NewValue = cmd.NewValue;
                OldValue = cmd.InitialValue;
                BaseValue = cmd.InitialValue;
                Difference = cmd.Difference;
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
