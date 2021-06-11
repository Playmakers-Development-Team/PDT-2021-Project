using Units;

namespace Commands
{
    public class UnitSelectedCommand : Command
    {
        public UnitSelectedCommand(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}