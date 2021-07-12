namespace Units.Commands
{
    public abstract class HistoricalCommand : UnitCommand
    {
        protected HistoricalCommand(IUnit unit) : base(unit) {}

        public abstract void Undo();
    }
}