using System;

namespace Units.Commands
{
    [Obsolete("Use StatChangedCommand instead")]
    public class ValueCommand : UnitCommand
    {
        public int Value { get; set; }
        
        protected ValueCommand(IUnit unit, int value) : base(unit) => Value = value;
    }
}
