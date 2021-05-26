using GridObjects;

namespace Units
{
    public abstract class UnitData
    {
        public int healthPoints;
        public int movementActionPoints;
        public int speed;
        public ModifierStat dealDamageModifier;
        public ModifierStat takeDamageModifier;
        public ModifierStat takeKnockbackModifier;

    }
}
