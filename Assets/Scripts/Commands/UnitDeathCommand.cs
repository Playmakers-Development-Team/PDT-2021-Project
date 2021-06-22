using Units;

namespace Commands
{
    public class UnitDeathCommand : Command
    {

        public IUnit Unit { get; }

        public UnitDeathCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}
