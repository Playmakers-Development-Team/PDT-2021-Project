using Units;

namespace Commands
{
    public class TurnQueueCreatedCommand : Command
    {
         public TurnQueueCreatedCommand(IUnit unit) : base(unit) {}

         public override void Queue() {}

         public override void Execute() {}

         public override void Undo() {}
    }
}
