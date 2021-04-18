using Units;

namespace Commands
{
    public abstract class Command
    {
        private IUnit unit;

        protected Command(IUnit unit)
        {
            this.unit = unit;
        }

        public abstract void Queue();

        public abstract void Execute();

        public abstract void Undo();
    }
}
