using Units;

namespace Commands
{
    public class ValueCommand : UnitCommand
    {
        public int Value { get; set; }

        protected ValueCommand(IUnit unit, int value) : base(unit) => this.Value = value;
        
        
    }
}
