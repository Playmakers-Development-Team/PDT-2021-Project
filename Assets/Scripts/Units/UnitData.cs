using System;
using GridObjects;

namespace Units
{
    public abstract class UnitData
    {
        public ValueStat healthPoints;
        public ValueStat movementActionPoints;
        public ValueStat speed;
        public ModifierStat dealDamageModifier;
        public ModifierStat takeDamageModifier;
        public ModifierStat takeKnockbackModifier;

        public void Initialise()
        {
            healthPoints.Reset();
            movementActionPoints.Reset();
            speed.Reset();
            dealDamageModifier.Reset();
            takeDamageModifier.Reset();
            takeKnockbackModifier.Reset();
        }
    }
}
