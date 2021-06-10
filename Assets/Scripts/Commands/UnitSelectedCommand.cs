using Managers;
using Units;

namespace Commands
{
    public class UnitSelectedCommand : Command
    {
        public IUnit CurrentUnit { get; }

        public UnitSelectedCommand(IUnit unit) 
        {
            CurrentUnit = unit;
        }
    }
}