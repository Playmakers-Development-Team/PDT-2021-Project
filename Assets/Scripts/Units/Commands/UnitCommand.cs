using Commands;

namespace Units.Commands
{
    public abstract class UnitCommand : Command
    {
        public IUnit Unit { get; }

        protected UnitCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}
