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
    /// Executed when a turn ends.
    /// </summary>
    public class EndTurnCommand : UnitCommand
    {
        public EndTurnCommand(IUnit unit) : base(unit) {}
    }

    /// <summary>
    /// Executed before a new round starts.
    /// </summary>
    public class PrepareRoundCommand : Command {}
    
    /// <summary>
    /// Executed after a new round starts.
    /// </summary>
    public class StartRoundCommand : Command {}
}