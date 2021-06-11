using Units;

namespace Commands
{
    public class UnitDeselectedCommand : Command
    {
        public UnitDeselectedCommand(IUnit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}
