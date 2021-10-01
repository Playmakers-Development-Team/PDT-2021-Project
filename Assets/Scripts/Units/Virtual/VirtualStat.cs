using Units.Stats;

namespace Units.Virtual
{
    public class VirtualStat
    {
        private readonly Stat stat;
        
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
        /// </summary>
        public int TotalValue => stat.Value + ValueDelta;

        /// <summary>
        /// The final base value after the change is applied.
        /// </summary>
        public int TotalBaseValue => stat.BaseValue + BaseValueDelta;

        public VirtualStat(Stat stat) => this.stat = stat;

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
    }
}
