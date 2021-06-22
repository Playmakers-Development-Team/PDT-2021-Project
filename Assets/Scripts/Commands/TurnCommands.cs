using Units;

namespace Commands
{
    /// <summary>
    /// Executed when the turn queue is created and ready.
    /// </summary>
    public class TurnQueueCreatedCommand : Command {}
    
    /// <summary>
    /// Executed when a turn is about to start.
    /// </summary>
    public class StartTurnCommand : UnitCommand
    {
        public StartTurnCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Should execute this command when the turn of unit should end.
    /// </summary>
    public class EndTurnCommand : UnitCommand
    {
        public EndTurnCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Should execute this command for adding to turn queue.
    /// </summary>
    public class AddUnitToTurnsCommand : UnitCommand
    {
        public AddUnitToTurnsCommand(IUnit unit) : base(unit) {}
    }
    
    /// <summary>
    /// Executed when a unit has been appended to the turn queue.
    /// </summary>
    public class UnitTurnsAddedCommand : UnitCommand
    {
        public UnitTurnsAddedCommand(IUnit unit) : base(unit) {}
    }

    /// <summary>
    /// Executed when the turn system is about to go the the next round.
    /// </summary>
    public class PrepareRoundCommand : Command {}
    
    /// <summary>
    /// Executed when the a new round has started.
    /// </summary>
    public class StartRoundCommand : Command {}
}