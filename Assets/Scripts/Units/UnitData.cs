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

        public void AddDefence(int amount) => throw new NotImplementedException();

        public void AddAttack(int amount) => throw new NotImplementedException();
    }
}
