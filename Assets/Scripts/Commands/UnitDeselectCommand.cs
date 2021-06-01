using Managers;
using Units;

namespace Commands
{
    public class UnitDeselectedCommand : Command
    {
        public IUnit CurrentUnit { get; }

        public UnitDeselectedCommand(IUnit unit) : base(unit) 
        {
            CurrentUnit = unit;
        }

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}
