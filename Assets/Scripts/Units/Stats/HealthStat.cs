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
        
        public int Value { get; set; }

        public HealthStat(KillUnitCommand unitDeathCommand, IUnit unit, int baseValue, StatTypes
                              statType) : base(unit, baseValue, statType)
        {
            this.unitDeathCommand = unitDeathCommand;
            Value = baseValue;
        }
    
        
        public int TakeDamage(int amount)
        {
            int initialDamageTaken = amount - unit.DefenceStat.Value;
            int calculatedDamageTaken = Mathf.Max(0, initialDamageTaken);
            commandManager.ExecuteCommand(new StatChangedCommand(unit, StatTypes.Health, Value,
                Value - calculatedDamageTaken));
            Value -= calculatedDamageTaken;
            CheckDeath();
            return calculatedDamageTaken;
        }
        
        public int HealDamage(int amount)
        {
            commandManager.ExecuteCommand(new StatChangedCommand(unit, StatTypes.Health, Value,
                Value + amount));
            Value += amount;
            return amount;
        }
        
        private void CheckDeath()
        {
            if (Value <= 0)
                ManagerLocator.Get<CommandManager>().ExecuteCommand(unitDeathCommand);
        }
    }
}
