using Units;

namespace Commands
{
    public class StartTurnCommand : Command
    {
        public StartTurnCommand(Unit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}