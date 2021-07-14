using System;
using Abilities;
using Abilities.Commands;
using Commands;
using Managers;
using Turn.Commands;
using Units;
using Units.Commands;
using UnityEngine;

namespace UI
{
    public class GameDialogue : Dialogue
    {
        internal readonly Event<IUnit> unitSelected = new Event<IUnit>();
        internal readonly Event unitDeselected = new Event();
        
        internal readonly Event<StatDifference> unitDamaged = new Event<StatDifference>();
        
        internal readonly Event<Ability> abilitySelected = new Event<Ability>();
        internal readonly Event abilityDeselected = new Event();
        internal readonly Event<Vector2> abilityRotated = new Event<Vector2>();
        internal readonly Event abilityConfirmed = new Event();
        
        internal readonly Event turnEnded = new Event();


        private CommandManager commandManager;


        internal IUnit SelectedUnit { get; private set; }
        
        internal Ability SelectedAbility { get; private set; }
        
        internal Vector2 AbilityDirection { get; private set; }
        
        
        #region MonoBehaviour Events

        internal override void OnAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();

            // Listen to Events
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
                commandManager.ExecuteCommand(new AbilityCommand(SelectedUnit, AbilityDirection, SelectedAbility));
            });
        }

        private void OnEnable()
        {
            // Subscribe to Commands
            commandManager.ListenCommand(typeof(AbilityCommand), OnAbilityConfirmed);
            commandManager.ListenCommand(typeof(EndTurnCommand), OnEndTurn);
            commandManager.ListenCommand((Action<TakeTotalDamageCommand>) OnUnitDamaged);
        }

        private void OnDisable()
        {
            // Unsubscribe from Commands
            commandManager.UnlistenCommand(typeof(AbilityCommand), OnAbilityConfirmed);
            commandManager.UnlistenCommand(typeof(EndTurnCommand), OnEndTurn);
            commandManager.UnlistenCommand((Action<TakeTotalDamageCommand>) OnUnitDamaged);
        }
        
        #endregion


        #region Command Listeners

        private void OnAbilityConfirmed()
        {
            abilityConfirmed.Invoke();
        }

        private void OnEndTurn()
        {
            turnEnded.Invoke();
        }

        private void OnUnitDamaged(TakeTotalDamageCommand cmd)
        {
            unitDamaged.Invoke(new StatDifference(cmd));
        }
        
        #endregion


        #region Dialogue Event Functions

        protected override void OnHide() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
        
        #endregion
        
        
        #region Structs
        
        #endregion
    }
}
