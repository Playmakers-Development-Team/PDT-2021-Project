namespace Unit.Commands
{
    public abstract class UnitCommand : Command.Command
    {
        public IUnit Unit { get; }

        protected UnitCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}
