using System.Collections.Generic;
using Abilities;
using Units.Stats;
using TenetStatuses;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units
{
    public abstract class UnitData
    {
        [SerializeField] private string name;
        [SerializeField] private TenetType tenet;
        [SerializeField] private ValueStat healthPoints;

        [FormerlySerializedAs("movementActionPoints")] [SerializeField]
        private Stat movementPoints;

        [SerializeField] private ValueStat speed;

        [FormerlySerializedAs("dealDamageModifier")] [SerializeField]
        private ModifierStat attack;

        [FormerlySerializedAs("takeDamageModifier")] [SerializeField]
        private ModifierStat defence;

        [SerializeField] private ModifierStat takeKnockbackModifier;
        [SerializeField] private List<Ability> abilities;
        
        [field: SerializeField] public HealthStat HealthValue { get; set; }
        [field: SerializeField] public Stat SpeedStat { get; set; }
        [field: SerializeField] public Stat DefenceStat { get; set; }
        [field: SerializeField] public Stat AttackStat { get; set; }
        [field: SerializeField] public Stat KnockbackStat { get; set; }
        
        [field: SerializeField] public List<TenetStatus> StartingTenets { get; set; }
        
        // TODO: Remove all fields of type "ModifierStat" or "ValueStat".
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

        public Stat MovementPoints
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

        //TODO: Remove this function.
        public void Initialise()
        {
            HealthPoints.Reset();
            Speed.Reset();
            Attack.Reset();
            Defence.Reset();
            TakeKnockbackModifier.Reset();
        }
    }
}
