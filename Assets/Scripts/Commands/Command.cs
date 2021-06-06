using Units;

namespace Commands
{
    public abstract class Command
    {
        public IUnit Unit { get; set; }

        protected Command(IUnit unit)
        {
            Unit = unit;
        }

        public abstract void Queue();

        public abstract void Execute();

        public abstract void Undo();
    }
}
