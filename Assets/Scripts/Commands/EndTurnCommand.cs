using Units;

namespace Commands
{
    public class EndTurnCommand : UnitCommand
    {
        public EndTurnCommand(IUnit unit) : base(unit) {}
    }
}