using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class HealthStat : Stat
    {
        private Action onUnitDeath;

        public HealthStat(Action onUnitDeath, IUnit unit, int baseValue, StatTypes
                              statType) : base(unit, baseValue, statType) =>
            this.onUnitDeath = onUnitDeath;

        public int TakeDamage(int amount)
        {
            int initialDamageTaken = amount - unit.DefenceStat.Value;
            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            
            // Decrease defence by amount of damage taken
            unit.DefenceStat.Value = Mathf.Max(0, unit.DefenceStat.Value - amount);
            
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
                onUnitDeath.Invoke();
        }
    }
}
