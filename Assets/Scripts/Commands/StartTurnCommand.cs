using Units;

namespace Commands
{
    public class StartTurnCommand : UnitCommand
    {
        public StartTurnCommand(IUnit unit) : base(unit) {}
    }
}