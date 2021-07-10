using System.Collections.Generic;
using Abilities;
using Commands;

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
    
    /// <summary>
    /// Executed when the total calculated amount of damage dealt to a unit is done.
    /// Taking into account attack and defence modifiers
    /// </summary>
    public class TakeTotalDamageCommand : ValueCommand
    {
        public TakeTotalDamageCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    /// <summary>
    /// Executed when damage is dealt to the player without any modifier consideration.
    /// </summary>
    public class TakeRawDamageCommand : ValueCommand
    {
        public TakeRawDamageCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the attack value of a unit is changed
    /// </summary>
    public class AttackChangeCommand : ValueCommand
    {
        public AttackChangeCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the attack value of a unit is changed
    /// </summary>
    public class DefenceChangeCommand : ValueCommand
    {
        public DefenceChangeCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the health value of a unit is changed
    /// </summary>
    public class HealthChangedCommand : ValueCommand
    {
        public HealthChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the speed value of a unit is changed
    /// </summary>
    public class SpeedChangedCommand : ValueCommand
    {
        public SpeedChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }

    /// <summary>
    /// Executed when the abilities list value of a unit is changed
    /// </summary>
    public class AbilitiesChangedCommand : UnitCommand
    {
        public List<Ability> Abilities { get; set; }
        public AbilitiesChangedCommand(IUnit unit, List<Ability> Abilities) : base(unit) => this.Abilities = Abilities;
    }
    
    /// <summary>
    /// Executed when the movement action points  value of a unit is changed
    /// </summary>
    public class MovementActionPointChangedCommand : ValueCommand
    {
        public MovementActionPointChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }
    
    /// <summary>
    /// Executed when the movement action points  value of a unit is changed
    /// </summary>
    public class KnockbackModifierChangedCommand : ValueCommand
    { 
        public KnockbackModifierChangedCommand(IUnit unit, int value) : base(unit, value) {}
    }
}
