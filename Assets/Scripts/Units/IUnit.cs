using GridObjects;

namespace Units
{
    public interface IUnit
    {
        public ModifierStat DealDamageModifier { get; }
        
        public int Speed { get; }
    }
}
