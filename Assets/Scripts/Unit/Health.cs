using System;
using Unit.Stats;
using UnityEngine;

namespace Unit
{
    public class Health
    {
        public ValueStat HealthPoints { get; }
        public ModifierStat TakeDamageModifier { get; }

        private Action OnDeath;

        public Health(Action onDeath, ValueStat healthPoints, ModifierStat takeDamageModifier)
        {
            OnDeath = onDeath;
            HealthPoints = healthPoints;
            TakeDamageModifier = takeDamageModifier;
        }
        
        public int TakeDamage(int amount)
        {
            int damageTaken = (int) TakeDamageModifier.Modify(amount);
            HealthPoints.Value -= damageTaken;
            CheckDeath();

            Debug.Log(damageTaken + " damage taken.");
            Debug.Log($"Health Before: {HealthPoints.Value + damageTaken}  |  Health After: {HealthPoints.Value}");

            return damageTaken;
        }
        
        private void CheckDeath()
        {
            if (HealthPoints.Value <= 0)
                OnDeath.Invoke();
        }
    }
}
