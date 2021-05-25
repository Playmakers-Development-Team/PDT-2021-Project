using System;
using Abilities;
using GridObjects;
using StatusEffects;

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
        
        public void Damage(int amount) => throw new NotImplementedException();

        public void Defend(int amount) => throw new NotImplementedException();

        public void Expend(TenetType tenet, int amount) => throw new NotImplementedException();

        public int GetStacks(TenetType tenet) => throw new NotImplementedException();
    }
}
