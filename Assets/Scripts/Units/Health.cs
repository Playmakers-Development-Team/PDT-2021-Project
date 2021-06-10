using System;
using GridObjects;
using UnityEngine;

namespace Units
{
    public class Health
    {
        public ValueStat HealthPoints { get; protected set; }
        public ModifierStat TakeDamageModifier { get; protected set; }

        private Action OnDeath;

        public Health(Action onDeath, ValueStat healthPoints, ModifierStat takeDamageModifier)
        {
            OnDeath = onDeath;
            HealthPoints = healthPoints;
            TakeDamageModifier = takeDamageModifier;
        }
        
        public int TakeDamage(int amount)
        {
            Debug.Log("Amount: " + amount);
            // Debug.Log("DamageModifier");
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
