using Units;

namespace Commands
{
    public class UnitKilledCommand : UnitCommand
    {
        public UnitKilledCommand(IUnit unit) : base(unit) {}
    }
    
    public class UnitSpawnCommand : UnitCommand
    {
        public UnitSpawnCommand(IUnit unit) : base(unit) {}
    }
}
