using Commands;
using GridObjects;
using Managers;
using UnityEngine;

namespace Units
{
    public class Health
    {
        public ValueStat HealthPoints { get; }
        public ModifierStat TakeDamageModifier { get; }

        private UnitDeathCommand unitDeathCommand;

        public Health(UnitDeathCommand unitDeathCommand, ValueStat healthPoints, ModifierStat 
        takeDamageModifier)
        {
            this.unitDeathCommand = unitDeathCommand;
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
            {
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
            }
        }
    }
}
