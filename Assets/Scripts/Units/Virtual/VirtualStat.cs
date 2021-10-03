using System;
using Units.Stats;

namespace Units.Virtual
{
    public class VirtualStat
    {
        /// <summary>
        /// Takes the current value and delta value to output a total value.
        /// </summary>
        public delegate int TotalValueModifierDelegate(int current, int delta);
        
        private readonly Stat stat;
        private readonly TotalValueModifierDelegate totalValueModifier;
        
        /// <summary>
        /// The change that is added on top of the base value.
        /// </summary>
        public int BaseValueDelta { get; set; }
        
        /// <summary>
        /// The change that is added on top of the value.
        /// </summary>
        public int ValueDelta { get; set; }

        /// <summary>
        /// Will be true, if there is a change in the stat.
        /// </summary>
        public bool HasDelta => ValueDelta != 0;

        /// <summary>
        /// The final value after the change is applied.
        /// A custom offset function may modify the final value after other stats are applied.
        /// </summary>
        public int TotalValue => totalValueModifier != null 
            ? totalValueModifier(stat.Value, ValueDelta) 
            : stat.Value + ValueDelta;

        /// <summary>
        /// The final base value after the change is applied.
        /// </summary>
        public int TotalBaseValue => stat.BaseValue + BaseValueDelta;

        public VirtualStat(Stat stat) => this.stat = stat;
        
        public VirtualStat(Stat stat, TotalValueModifierDelegate totalValueModifier)
        {
            this.stat = stat;
            this.totalValueModifier = totalValueModifier;
        }

        /// <summary>
        /// Apply the change in values into the stat. The deltas are reset to 0 after they are applied.
        /// </summary>
        public void SetValues()
        {
            if (BaseValueDelta != 0)
                stat.BaseValue += BaseValueDelta;

            if (ValueDelta != 0)
                stat.Value += ValueDelta;
            
            // We should clear the delta after we apply the changes
            BaseValueDelta = 0;
            ValueDelta = 0;
        }

        public void ResetValues() => stat.Reset();
    }
}
