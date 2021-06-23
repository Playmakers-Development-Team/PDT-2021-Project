using Units;

namespace Commands
{
    // TODO: Should inherit from UnitCommand when available
    // TODO: Will also need to be moved to the Unit system
    public class UnitDeathCommand : Command
    {
        public IUnit Unit { get; }

        public UnitDeathCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}
