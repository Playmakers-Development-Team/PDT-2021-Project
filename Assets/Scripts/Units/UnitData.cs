using System;
using Abilities;
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
        
        public void Damage(int amount) => throw new NotImplementedException();

        public void Defend(int amount) => throw new NotImplementedException();

        public void Expend(Tenet tenet, int amount) => throw new NotImplementedException();

        public int GetStacks(Tenet tenet) => throw new NotImplementedException();
    }
}
