using Units;

namespace Commands
{
    public class EndTurnCommand : Command
    {
        public IUnit Unit { get; }
        
        public EndTurnCommand(IUnit unit)
        {
            Unit = unit;
        }
    }
}