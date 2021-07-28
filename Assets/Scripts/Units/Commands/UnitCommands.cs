using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Units.Stats;

namespace Units.Commands
{
    /// <summary>
    /// Executed when a unit should be killed and removed from the game.
    /// Specifically, this command is useful for debugging from editor.
    /// </summary>
    public class KillUnitCommand : UnitCommand
    {
        public KillUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit is about to be removed from the game.
    /// </summary>
    public class KillingUnitCommand : UnitCommand
    {
        public KillingUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit has been removed from the game.
    /// </summary>
    public class KilledUnitCommand : UnitCommand
    {
        public KilledUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit is about to be spawned.
    /// </summary>
    public class SpawningUnitCommand : Command {}
    
    /// <summary>
    /// Executed when a unit has been spawned.
    /// </summary>
    public class SpawnedUnitCommand : UnitCommand
    {
        public SpawnedUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit is no longer selected by the player.
    /// </summary>
    public class UnitDeselectedCommand : UnitCommand
    {
        public UnitDeselectedCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit is selected by the player.
    /// </summary>
    public class UnitSelectedCommand : UnitCommand
    {
        public UnitSelectedCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when all player units are spawned and ready in the scene.
    /// </summary>
    public class PlayerUnitsReadyCommand : Command {}

    /// <summary>
    /// Executed when all enemy units are spawned and ready in the scene.
    /// </summary>
    public class EnemyUnitsReadyCommand : Command {}


    public class StatChangedCommand : UnitCommand
    {
        public StatTypes StatType { get; }
        
        public int BaseValue { get; }
        
        public int Value { get;}
        
        public int NewValue { get; }

        public StatChangedCommand(IUnit unit, StatTypes type, int baseValue, int amount,
                                  int newValue) : base(unit)
        {
            StatType = type;
            BaseValue = baseValue;
            Value = amount;
            NewValue = newValue;
        }
    }
    
    /// <summary>
    /// Executed when damage is dealt to a unit. Value takes into account attack and defence modifiers.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class TakeTotalDamageCommand : ValueCommand
    {
        public TakeTotalDamageCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    /// <summary>
    /// Executed when damage is dealt to a unit. Value does not take modifiers into consideration.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class TakeRawDamageCommand : ValueCommand
    {
        public TakeRawDamageCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the attack value of a unit is changed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class AttackChangeCommand : ValueCommand
    {
        public AttackChangeCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the defence value of a unit is changed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class DefenceChangeCommand : ValueCommand
    {
        public DefenceChangeCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    /// <summary>
    /// Executed when the speed value of a unit is changed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class SpeedChangedCommand : ValueCommand
    {
        public SpeedChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the abilities list of a unit is changed.
    /// </summary>
    public class AbilitiesChangedCommand : UnitCommand
    {
        public List<Ability> Abilities { get; set; }
        public AbilitiesChangedCommand(IUnit unit, List<Ability> Abilities) : base(unit) => this.Abilities = Abilities;
    }
    
    /// <summary>
    /// Executed when the movement action points value of a unit is changed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class MovementActionPointChangedCommand : ValueCommand
    {
        public MovementActionPointChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    /// <summary>
    /// Executed when the knockback modifer value of a unit is changed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class KnockbackModifierChangedCommand : ValueCommand
    { 
        public KnockbackModifierChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    // TODO: Can be deleted once enemy abilities are implemented
    /// <summary>
    /// Executed when the an enemy unit attacks.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class EnemyAttack : UnitCommand
    {
        public EnemyAttack(IUnit unit, IUnit playerUnit, int amount) : base(unit) =>
            playerUnit.TakeDamage(amount);
    }

    /// <summary>
    /// Executed when the an enemy unit casting animation has completed.
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class EndUnitCastingCommand : UnitCommand
    {
        public EndUnitCastingCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when all the enemy logic has been completed.
    /// Hence, we may be free to proceed to the next turn
    /// </summary>
    [Obsolete("Use StatChangedCommand instead")]
    public class EnemyActionsCompletedCommand : UnitCommand
    {
        public EnemyActionsCompletedCommand(IUnit unit) : base(unit) {}
    }

    /// <summary>
    /// Executed when a generic UnitManager is ready to accept IUnit spawns.
    /// </summary>
    public class UnitManagerReadyCommand<T> : Command where T : UnitData {}
}
