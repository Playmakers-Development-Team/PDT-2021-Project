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

        private readonly KillUnitCommand unitDeathCommand;
        private readonly TakeRawDamageCommand takeRawDamageCommand;

        private readonly CommandManager commandManager;
        
        public Health(TakeRawDamageCommand takeRawDamageCommand, KillUnitCommand unitDeathCommand,
                      ValueStat healthPoints, ModifierStat defence)
        {
            this.takeRawDamageCommand = takeRawDamageCommand;
            this.unitDeathCommand = unitDeathCommand;
            HealthPoints = healthPoints;
            Defence = defence;
            commandManager = ManagerLocator.Get<CommandManager>();
        }
        
        public int TakeDamage(int amount)
        {
            int initialDamageTaken = (int) Defence.Modify(amount);
            takeRawDamageCommand.Value = initialDamageTaken;
            commandManager.ExecuteCommand(takeRawDamageCommand);

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
