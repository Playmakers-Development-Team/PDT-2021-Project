using Managers;
using Units;

namespace Commands
{
    public class MoveCommand : Command
    {
        public MoveCommand(Unit unit) : base(unit) {}

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}
