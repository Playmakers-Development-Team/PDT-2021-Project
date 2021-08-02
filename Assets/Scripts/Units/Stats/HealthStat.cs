using System;
using Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class HealthStat : Stat
    {
        private readonly KillUnitCommand unitDeathCommand;
        private Action OnUnitDeath;

        public HealthStat(Action onUnitDeath, IUnit unit, int baseValue, StatTypes
                              statType) : base(unit, baseValue, statType) =>
            OnUnitDeath = onUnitDeath;

        public int TakeDamage(int amount)
        {
            int initialDamageTaken = amount - unit.DefenceStat.Value;
            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            
            Value -= calculatedDamageTaken;
            CheckDeath();
            return calculatedDamageTaken;
        }
        
        public int HealDamage(int amount)
        {
            Value += amount;
            return amount;
        }
        
        private void CheckDeath()
        {
            if (Value <= 0)
                OnUnitDeath.Invoke();
        }
    }
}
