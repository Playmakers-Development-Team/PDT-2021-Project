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
        
        public HealthStat(KillUnitCommand unitDeathCommand,IUnit unit, int baseValue, StatTypes 
        statType) : base(unit, baseValue, statType)
        => this.unitDeathCommand = unitDeathCommand;
        
        public int TakeDamage(int amount)
        {
            
            int initialDamageTaken = amount - unit.DefenceStat.Value;
            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            Value -= calculatedDamageTaken;
            CheckDeath();
            Debug.Log(calculatedDamageTaken + " damage taken.");
            Debug.Log($"Health Before: {Value + calculatedDamageTaken}  |  Health After: {Value}");
            commandManager.ExecuteCommand(new StatChangedCommand(unit,calculatedDamageTaken,StatTypes.Health));
            return calculatedDamageTaken;
        }
        
        public int HealDamage(int amount)
        {
            Value += amount;
            Debug.Log(amount + " health gained!.");
            Debug.Log($"Health Before: {Value - amount}  |  Health After: {Value}");
            commandManager.ExecuteCommand(new StatChangedCommand(unit,amount,StatTypes.Health));
            return amount;
        }
        
        
        private void CheckDeath()
        {
            if (Value <= 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
        }
        
        
        
        
        
    }
}
