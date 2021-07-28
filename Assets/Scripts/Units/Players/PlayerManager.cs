using System;
using Units.Stats;

namespace Units.Players
{
    public class PlayerManager : UnitManager<PlayerUnitData>
    {
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay { get; } = 1000;

        [Obsolete("Use Insight of type 'Stat' instead from the TurnManager")]
        public ValueStat Insight { get; private set; }
        
        public override void ManagerStart()
        {
            base.ManagerStart();
            
            // TODO: The base value of insight might want to be exposed somewhere
            // TODO: ValueStat constructor should take BaseValue as an argument
            Insight = new ValueStat {BaseValue = 0};
            Insight.Reset();
        }
    }
}
