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
    /// Executed when all units of type T are spawned and ready in the scene.
    /// </summary>
    public class UnitsReadyCommand<T> : Command where T : UnitData {}

    public class StatChangedCommand : UnitCommand
    {
        public StatTypes StatType { get; }
        
        public int InitialValue { get; }
        
        public int Difference { get; }
        
        public int MaxValue { get; }
        
        public int DisplayValue { get;}
        public int NewValue { get; }

        public StatChangedCommand(IUnit unit, StatTypes type, int maxValue,int initialValue, int 
        newValue) 
        : base(unit)
        {
            StatType = type;
            InitialValue = initialValue;
            NewValue = newValue;
            Difference = newValue - initialValue;
            DisplayValue = Math.Abs(Difference);
            MaxValue = maxValue;
        }
    }

    /// <summary>
    /// Executed when the abilities list of a unit is changed.
    /// </summary>
    public class AbilitiesChangedCommand : UnitCommand
    {
        public List<Ability> Abilities { get; set; }
        public AbilitiesChangedCommand(IUnit unit, List<Ability> Abilities) : base(unit) => this.Abilities = Abilities;
    }

    // TODO: Should be a more generic EndAnimationCommand.
    /// <summary>
    /// Executed when the an enemy unit casting animation has completed.
    /// </summary>
    public class EndUnitCastingCommand : UnitCommand
    {
        public EndUnitCastingCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when all the enemy logic has been completed.
    /// Hence, we may be free to proceed to the next turn
    /// </summary>
    public class EnemyActionsCompletedCommand : UnitCommand
    {
        public EnemyActionsCompletedCommand(IUnit unit) : base(unit) {}
    }

    /// <summary>
    /// Executed when a generic UnitManager is ready to accept IUnit spawns.
    /// </summary>
    public class UnitManagerReadyCommand<T> : Command where T : UnitData {}
}
