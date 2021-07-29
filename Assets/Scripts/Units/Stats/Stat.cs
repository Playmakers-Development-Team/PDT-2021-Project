using System;
using System.Linq;
using Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class Stat
    {
       public int Value
        {
            get => value;
            set
            {
                commandManager.ExecuteCommand(new StatChangedCommand(
                    unit,
                    StatType,
                    this.value,
                    value
                ));
                
                this.value = value;
            }
        }
       
        [field: SerializeField] public int BaseValue { get; set; }
        public StatTypes StatType { get; private set; }

        public readonly CommandManager commandManager; 

        public readonly IUnit unit;
        
        private int value;

        public Stat(IUnit unit,int baseValue, StatTypes statType)
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            BaseValue = baseValue;
            Value = baseValue;
            StatType = statType;
            this.unit = unit;
        }

        public void Reset() => Value = BaseValue;
    }
}
