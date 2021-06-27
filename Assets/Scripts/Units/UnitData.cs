using System;
using System.Collections.Generic;
using Abilities;
using GridObjects;
using StatusEffects;
using UnityEngine;

namespace Units
{
    [Serializable]
    public abstract class UnitData
    {
        public TenetType tenet;
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
