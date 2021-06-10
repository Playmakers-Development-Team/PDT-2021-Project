using Units;

namespace Commands
{
    public abstract class HistoricalCommand : Command
    {
        public abstract void Undo();
    }
}