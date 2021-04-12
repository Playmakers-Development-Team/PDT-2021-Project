namespace Commands
{
    public abstract class Command
    {
        //*protected IUnit unit;

        //* public Command (IUnit unit)
        //{

        //} 

        public abstract void Execute();

        public abstract void Undo();
    }
}
