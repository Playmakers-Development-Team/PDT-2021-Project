using Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Stats
{
    public class HealthStat : Stat
    {
        private readonly KillUnitCommand unitDeathCommand;
        
        public HealthStat(KillUnitCommand unitDeathCommand,IUnit unit, int baseValue, StatTypes 
        statType) : base(unit, baseValue, statType)
        => this.unitDeathCommand = unitDeathCommand;
        
        public int TakeDamage(int amount, Stat defenceModifier)
        {
            int initialDamageTaken = amount - defenceModifier.Value;
            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            Value -= calculatedDamageTaken;
            CheckDeath();

            Debug.Log(calculatedDamageTaken + " damage taken.");
            Debug.Log($"Health Before: {Value + calculatedDamageTaken}  |  Health After: {Value}");

            return calculatedDamageTaken;
        }
        
        private void CheckDeath()
        {
            if (Value <= 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
        }
        
        
        
        
        
    }
}
