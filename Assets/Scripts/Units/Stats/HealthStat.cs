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
            commandManager.ExecuteCommand(new StatChangedCommand(unit,StatTypes.Health,Value + 
            calculatedDamageTaken,calculatedDamageTaken,Value));
            return calculatedDamageTaken;
        }
        
        public int HealDamage(int amount)
        {
            Value += amount;
            commandManager.ExecuteCommand(new StatChangedCommand(unit,StatTypes.Health,Value - 
            amount,amount,Value));
            return amount;
        }
        
        private void CheckDeath()
        {
            if (Value <= 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
        }
    }
}
