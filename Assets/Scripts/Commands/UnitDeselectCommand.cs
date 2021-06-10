using Managers;
using Units;

namespace Commands
{
    public class UnitDeselectedCommand : Command
    {
        public IUnit CurrentUnit { get; }

        public UnitDeselectedCommand(IUnit unit)
        {
            CurrentUnit = unit;
        }
    }
}
