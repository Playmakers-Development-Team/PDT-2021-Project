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
}
