using Units;

namespace Commands
{
    public class StartTurnCommand : Command
    {
        public IUnit Unit { get; }
        
        public StartTurnCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}