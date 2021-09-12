using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using Turn;
using Turn.Commands;
using UI.Core;
using UI.Game.UnitPanels.Abilities;
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
        
        internal readonly Event<StatChangeInfo> unitStatChanged = new Event<StatChangeInfo>();
        internal readonly Event<TenetChangeInfo> unitTenetChanged = new Event<TenetChangeInfo>();
        
        internal readonly Event<Ability> abilitySelected = new Event<Ability>();
        internal readonly Event<Ability> abilityDeselected = new Event<Ability>();
        internal readonly Event<AbilityCard> abilityHoverEnter = new Event<AbilityCard>();
        internal readonly Event<AbilityCard> abilityHoverExit = new Event<AbilityCard>();
        internal readonly Event<Vector2> abilityRotated = new Event<Vector2>();
        internal readonly Event abilityConfirmed = new Event();
        
        internal readonly Event<TurnInfo> turnStarted = new Event<TurnInfo>();
        
        internal readonly Event<TurnInfo> turnManipulated = new Event<TurnInfo>();


        internal readonly Event<UnitInfo> meditateConfirmed = new Event<UnitInfo>();
        internal readonly Event<MoveInfo> moveConfirmed = new Event<MoveInfo>();
        internal readonly Event buttonSelected = new Event();
        
        internal readonly Event<Mode> modeChanged = new Event<Mode>();

        private CommandManager commandManager;
        private TurnManager turnManager;
        
        private readonly List<UnitInfo> units = new List<UnitInfo>();
        
        
        internal UnitInfo SelectedUnit { get; private set; }
        
        internal Ability SelectedAbility { get; private set; }
        
        internal Vector2 AbilityDirection { get; private set; }
        
        internal Mode DisplayMode { get; private set; }
        
        
        internal enum Mode
        {
            Default,
            Aiming,
            Moving
        }
        
        
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
            
            turnStarted.AddListener(info =>
            {
                abilityDeselected.Invoke(SelectedAbility);
            });

            moveConfirmed.AddListener(info =>
            {
                commandManager.ExecuteCommand(new StartMoveCommand(info.UnitInfo.Unit, info.Destination));
            });
            
            modeChanged.AddListener(mode =>
            {
                DisplayMode = mode;
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
            commandManager.ListenCommand((Action<TurnManipulatedCommand>) OnTurnManipulated);
        }

        private void OnDisable()
        {
            // Unsubscribe from Commands
            commandManager.UnlistenCommand((Action<StartTurnCommand>) OnStartTurn);
            commandManager.UnlistenCommand((Action<StartMoveCommand>) OnStartMove);
            commandManager.UnlistenCommand((Action<EndMoveCommand>) OnEndMove);
            commandManager.UnlistenCommand((Action<StatChangedCommand>) OnUnitDamaged);
            commandManager.UnlistenCommand((Action<KilledUnitCommand>) OnUnitKilled);
            commandManager.UnlistenCommand((Action<TurnManipulatedCommand>) OnTurnManipulated);

        }
        
        #endregion


        #region Command Listeners

        private void OnStartTurn(StartTurnCommand cmd)
        {
            if (cmd.Unit == null)
                return;
            
            UnitInfo info = GetInfo(cmd.Unit);

            turnStarted.Invoke(new TurnInfo(info, turnManager.ActingPlayerUnit != null));
        }

        private void OnTurnManipulated(TurnManipulatedCommand cmd)
        {
            UnitInfo info = GetInfo(cmd.Unit);

            if (info == null)
                throw new Exception("ActingUnit was not in GameDialogue.units.");
            
            turnManipulated.Invoke(new TurnInfo(info, turnManager.ActingPlayerUnit != null));
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
            unitStatChanged.Invoke(new StatChangeInfo(cmd));
        }
        
        private void OnUnitKilled(KilledUnitCommand cmd)
        {
            UnitInfo info = GetInfo(cmd.Unit);

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

        internal UnitInfo GetInfo(IUnit unit)
        {
            if (units.Count == 0)
            {
                throw new Exception($"Could not get {nameof(UnitInfo)} for {unit}. " +
                                    $"{nameof(GameDialogue)}.{nameof(units)} is empty.");
            }

            var unitInfo = units.Find(u => u.Unit == unit);
                
            if (unitInfo == null)
            {
                throw new Exception($"Could not get {nameof(UnitInfo)} for {unit}. " +
                                    $"{unit} is not in {nameof(GameDialogue)}.{nameof(units)}.");
            }

            return unitInfo;
        }

        #endregion
        
        
        #region Structs

        // TODO: Turn this into a struct, null comparison can be made on UnitInfo.Unit...
        [Serializable]
        public class UnitInfo
        {
            [SerializeField] private CropInfo profileCropInfo;
            [SerializeField] private CropInfo timelineCropInfo;

            
            internal CropInfo ProfileCropInfo => profileCropInfo;
            internal CropInfo TimelineCropInfo => timelineCropInfo;
            
            public IUnit Unit { get; private set; }


            internal void SetUnit(IUnit newUnit) => Unit = newUnit;
        }

        internal readonly struct TurnInfo
        {
            internal UnitInfo CurrentUnit { get; }
            internal bool IsPlayer { get; }


            public TurnInfo(UnitInfo currentUnit, bool isPlayer)
            {
                CurrentUnit = currentUnit;
                IsPlayer = isPlayer;
            }
        }
        
        internal readonly struct StatChangeInfo
        {
            internal IUnit Unit { get; }
            internal int NewValue { get; }
            internal int OldValue { get; }
            internal int BaseValue { get; }
            internal int Difference { get; }
            internal int DisplayValue { get; }
            internal StatTypes  StatType { get; }

            internal StatChangeInfo(StatChangedCommand cmd)
            {
                StatType = cmd.StatType;
                Unit = cmd.Unit;
                NewValue = cmd.NewValue;
                OldValue = cmd.InitialValue;
                BaseValue = cmd.MaxValue;
                Difference = cmd.Difference;
                DisplayValue = cmd.DisplayValue;
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
        
        internal readonly struct TenetChangeInfo
        {
            internal IUnit Unit { get; }
            internal int NewValue { get; }
            internal int OldValue { get; }
            internal int BaseValue { get; }
            internal int Difference { get; }
            internal int DisplayValue { get; }
            internal TenetType  TenetType { get; }

            // internal TenetChangeInfo(StatChangedCommand cmd)
            // {
            //     TenetType = cmd.StatType;
            //     Unit = cmd.Unit;
            //     NewValue = cmd.NewValue;
            //     OldValue = cmd.InitialValue;
            //     BaseValue = cmd.MaxValue;
            //     Difference = cmd.Difference;
            //     DisplayValue = cmd.DisplayValue;
            // }
        }
        
        #endregion
    }
}
