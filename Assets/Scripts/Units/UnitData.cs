using System;
using GridObjects;

namespace Units
{
    public abstract class UnitData
    {
        public int healthPoints;
        public int movementActionPoints;
        public int speed;
        public Stat dealDamageModifier;
        public Stat takeDamageModifier;
        public Stat takeKnockbackModifier;
    }
}
