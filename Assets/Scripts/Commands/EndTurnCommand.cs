using Units;

namespace Commands
{
    public class EndTurn : Command
    {
        public EndTurn(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}