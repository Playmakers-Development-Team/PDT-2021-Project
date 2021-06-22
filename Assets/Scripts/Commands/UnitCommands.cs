using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    /// <summary>
    /// Executed when unit is to be killed and about to be removed from the game.
    /// </summary>
    public class KillingUnitCommand : UnitCommand
    {
        public KillingUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when unit has already been removed completely from the game.
    /// </summary>
    public class KilledUnitCommand : UnitCommand
    {
        public KilledUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when unit is about to be spawned.
    /// </summary>
    public class SpawningUnitCommand : UnitCommand
    {
        public SpawningUnitCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when unit has already been completely spawned.
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
}
