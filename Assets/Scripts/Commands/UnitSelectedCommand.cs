using Managers;
using Units;

namespace Commands
{
    public class UnitSelectedCommand : Command
    {
        public IUnit CurrentUnit { get; }

        public UnitSelectedCommand(IUnit unit) : base(unit) 
        {
            CurrentUnit = unit;
        }

        public override void Queue() {}

        public override void Execute() {}

        public override void Undo() {}
    }
}   