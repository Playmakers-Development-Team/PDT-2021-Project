using Units;

namespace Commands
{
    public abstract class Command
    {
        protected Unit unit;

        protected Command(Unit unit)
        {
            this.unit = unit;
        }

        public abstract void Queue();

        public abstract void Execute();

        public abstract void Undo();
    }
}
