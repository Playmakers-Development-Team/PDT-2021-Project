using System;
using UnityEngine;
using System.Collections.Generic;
using Abilities;
using GridObjects;
using StatusEffects;
using UnityEngine.Serialization;

namespace Units
{
    public abstract class UnitData
    {
        [SerializeField] private string name;
        [SerializeField] private TenetType tenet;
        [SerializeField] private ValueStat healthPoints;
        [FormerlySerializedAs("movementActionPoints")]
        [SerializeField] private ValueStat movementPoints;
        [SerializeField] private ValueStat speed;
        [FormerlySerializedAs("dealDamageModifier")]
        [SerializeField] private ModifierStat attack;
        [FormerlySerializedAs("takeDamageModifier")]
        [SerializeField] private ModifierStat defence;
        [SerializeField] private ModifierStat takeKnockbackModifier;
        [SerializeField] private List<Ability> abilities;
        
        public string Name
        {
            get => name;
            set => name = value;
        }
        public TenetType Tenet => tenet;
        public ValueStat HealthPoints => healthPoints;
        public ValueStat MovementPoints => movementPoints;
        public ValueStat Speed => speed;
        public ModifierStat Attack => attack;
        public ModifierStat Defence => defence;
        public ModifierStat TakeKnockbackModifier => takeKnockbackModifier;
        public List<Ability> Abilities => abilities;

        public void Initialise()
        {
            HealthPoints.Reset();
            MovementPoints.Reset();
            Speed.Reset();
            Attack.Reset();
            Defence.Reset();
            TakeKnockbackModifier.Reset();
        }
    }
}
