using System;
using UnityEngine;
using System.Collections.Generic;
using Abilities;
using GridObjects;
using Units.Stats;
using Units.TenetStatuses;
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
        
        // TODO: Change these to be serialized auto properties 
        public string Name
        {
            get => name;
            set => name = value;
        }

        public TenetType Tenet
        {
            get => tenet;
            set => tenet = value;
        } 

        public ValueStat HealthPoints
        {
            get => healthPoints;
            set => healthPoints = value;
        } 
        public ValueStat MovementPoints
        {
            get => movementPoints;
            set => movementPoints = value;
        }

        public ValueStat Speed
        {
            get => speed;
            set => speed = value;
        } 
    
        public ModifierStat Attack
        {
            get => attack;
            set => attack = value;
        }

        public ModifierStat Defence
        {
            get => defence;
            set => defence = value;
        }

        public ModifierStat TakeKnockbackModifier
        {
            get => takeKnockbackModifier;
            set => takeKnockbackModifier = value;
        }

        public List<Ability> Abilities
        {
            get => abilities;
            set => abilities = value;
        }

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
