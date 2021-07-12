using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Stats
{
    public class Health
    {
        public ValueStat HealthPoints { get; }
        public ModifierStat Defence { get; }

        private readonly KillUnitCommand unitDeathCommand;

        private readonly CommandManager commandManager;
        
        public Health( KillUnitCommand unitDeathCommand,
                      ValueStat healthPoints, ModifierStat defence)
        {
            this.unitDeathCommand = unitDeathCommand;
            HealthPoints = healthPoints;
            Defence = defence;
            commandManager = ManagerLocator.Get<CommandManager>();
        }
        
        public int TakeDamage(int amount)
        {
            int initialDamageTaken = (int) Defence.Modify(amount);

            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            
            HealthPoints.Value -= calculatedDamageTaken;
            CheckDeath();

            Debug.Log(calculatedDamageTaken + " damage taken.");
            Debug.Log($"Health Before: {HealthPoints.Value + calculatedDamageTaken}  |  Health After: {HealthPoints.Value}");

            return calculatedDamageTaken;
        }
        
        private void CheckDeath()
        {
            if (HealthPoints.Value <= 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
        }
    }
}
