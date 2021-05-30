using Units;

namespace Commands
{
    public class EndTurnCommand : Command
    {
        public EndTurnCommand(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}