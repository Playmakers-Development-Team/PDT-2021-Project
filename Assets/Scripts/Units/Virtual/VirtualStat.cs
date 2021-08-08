using Units.Stats;

namespace Units.Virtual
{
    public class VirtualStat
    {
        private readonly Stat stat;
        
        public int BaseValueOffset { get; set; }
        
        public int ValueOffset { get; set; }

        public int TotalValue => stat.Value + ValueOffset;

        public int TotalBaseValue => stat.BaseValue + BaseValueOffset;

        public VirtualStat(Stat stat) => this.stat = stat;

        public void SetValues()
        {
            if (BaseValueOffset != 0)
                stat.BaseValue += BaseValueOffset;

            if (ValueOffset != 0)
                stat.Value += ValueOffset;
        }
    }
}
