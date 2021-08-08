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

        [FormerlySerializedAs("movementActionPoints")] [SerializeField]
        private Stat movementPoints;

        [SerializeField] private List<Ability> abilities;
        
        [field: SerializeField] public HealthStat HealthValue { get; set; }
        [field: SerializeField] public Stat SpeedStat { get; set; }
        [field: SerializeField] public Stat DefenceStat { get; set; }
        [field: SerializeField] public Stat AttackStat { get; set; }
        [field: SerializeField] public Stat KnockbackStat { get; set; }
        
        [field: SerializeField] public List<TenetStatus> StartingTenets { get; set; }
        
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

        public Stat MovementPoints
        {
            get => movementPoints;
            set => movementPoints = value;
        }

        public List<Ability> Abilities
        {
            get => abilities;
            set => abilities = value;
        }
    }
}
