using System.Collections.Generic;
using Abilities;
using Units.Stats;
using TenetStatuses;
using UnityEngine;

namespace Units
{
    public abstract class UnitData
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public TenetType Tenet { get; set; }
        [field: SerializeField] public Stat MovementPoints { get; set; }
        [field: SerializeField] public List<Ability> Abilities { get; set; }
        [field: SerializeField] public HealthStat HealthValue { get; set; }
        [field: SerializeField] public Stat SpeedStat { get; set; }
        [field: SerializeField] public Stat DefenceStat { get; set; }
        [field: SerializeField] public Stat AttackStat { get; set; }
        [field: SerializeField] public Stat KnockbackStat { get; set; }
        [field: SerializeField] public List<TenetStatus> StartingTenets { get; set; }
    }
}
