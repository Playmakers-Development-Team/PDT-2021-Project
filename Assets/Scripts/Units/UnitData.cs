using System;
using UnityEngine;
using System.Collections.Generic;
using Abilities;
using GridObjects;
using StatusEffects;

namespace Units
{
    public abstract class UnitData
    {
        public string name;
        private TenetType tenet;
        private ValueStat healthPoints;
        private ValueStat movementActionPoints;
        private ValueStat speed;
        private ModifierStat dealDamageModifier;
        private ModifierStat takeDamageModifier;
        private ModifierStat takeKnockbackModifier;
        private List<Ability> abilities;
        public TenetType Tenet => tenet;
        public ValueStat HealthPoints => healthPoints;
        public ValueStat MovementActionPoints => movementActionPoints;
        public ValueStat Speed => speed;
        public ModifierStat DealDamageModifier => dealDamageModifier;
        public ModifierStat TakeDamageModifier => takeDamageModifier;
        public ModifierStat TakeKnockbackModifier => takeKnockbackModifier;
        public List<Ability> Abilities => abilities;
       
        public void Initialise()
        {
            HealthPoints.Reset();
            MovementActionPoints.Reset();
            Speed.Reset();
            DealDamageModifier.Reset();
            TakeDamageModifier.Reset();
            TakeKnockbackModifier.Reset();
        }
    }
}
