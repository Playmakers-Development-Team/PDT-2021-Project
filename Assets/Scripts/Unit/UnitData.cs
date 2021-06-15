using System.Collections.Generic;
using Unit.Abilities;
using Unit.Stats;

namespace Unit
{
    public abstract class UnitData
    {
        public ValueStat healthPoints;
        public ValueStat movementActionPoints;
        public ValueStat speed;
        public ModifierStat dealDamageModifier;
        public ModifierStat takeDamageModifier;
        public ModifierStat takeKnockbackModifier;
        public List<Ability> abilities;
       
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
