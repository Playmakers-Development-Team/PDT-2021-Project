using Commands;
using Units;
using Units.Commands;

namespace Turn.Commands
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

    /// <summary>
    /// Executed when the order of the turn queue is changed.
    /// </summary>
    public class TurnManipulatedCommand : UnitCommand
    {
        public IUnit TargetUnit { get; private set; }

        public TurnManipulatedCommand(IUnit unit, IUnit targetUnit) : base(unit) =>
            TargetUnit = targetUnit;
    }

    public class MeditatedCommand : UnitCommand
    {
        public MeditatedCommand(IUnit unit) : base(unit) {}
    }

    
    public class GameEndedCommand : Command
    {
        public bool DidPlayerWin { get; set; }
        public GameEndedCommand(bool didPlayerWin) => DidPlayerWin = didPlayerWin;
        
    }


    
    
}