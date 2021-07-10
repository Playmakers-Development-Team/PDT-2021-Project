using Units;

namespace Commands
{
    public class ValueCommand : UnitCommand
    {
        public int Value { get; set; }
        
        protected ValueCommand(IUnit unit) : base(unit) {}
        
    }
}
