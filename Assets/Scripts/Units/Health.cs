using Commands;
using GridObjects;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units
{
    public class Health
    {
        public ValueStat HealthPoints { get; }
        public ModifierStat Defence { get; }

        private KillUnitCommand unitDeathCommand;

        public Health(KillUnitCommand unitDeathCommand, ValueStat healthPoints, ModifierStat 
        defence)
        {
            this.unitDeathCommand = unitDeathCommand;
            HealthPoints = healthPoints;
            Defence = defence;
        }
        
        public int TakeDamage(int amount)
        {
            int damageTaken = (int) Defence.Modify(amount);
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
