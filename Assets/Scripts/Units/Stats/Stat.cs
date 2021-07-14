using System.Linq;
using Commands;
using Managers;
using Units.Commands;

namespace Units.Stats
{
    public class Stat
    {
        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                commandManager.ExecuteCommand(new StatChangedCommand(unit, value, StatType));
            }
        }
        public int BaseValue { get; set; }
        public StatTypes StatType { get; private set; }

        private readonly CommandManager commandManager;
        
        private IUnit unit;
        private int value;

        public Stat(IUnit unit,int baseValue, StatTypes statType)
        {
            BaseValue = baseValue;
            Value = baseValue;
            StatType = statType;
            this.unit = unit;
            ManagerLocator.Get<CommandManager>();
        }

        public void Reset() => Value = BaseValue;
        
        
        
        
        
        
    }
}
