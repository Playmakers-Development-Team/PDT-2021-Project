using System;
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
                    StatType,BaseValue,
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

        /// <summary>
        /// Reduce this stat's value closer to the base value. We might want to do this maybe after a round
        /// ends if that is how the mechanic will be set up.
        /// </summary>
        public void ReduceToBaseValue() => 
            Value = Mathf.Max(BaseValue, Value - 2);

        public override string ToString() => $"{Value}/{BaseValue}";
    }
}
