using System;
using Units.Stats;

namespace Units.Players
{
    public class PlayerManager : UnitManager<PlayerUnitData>
    {
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay { get; } = 1000;
    }
}
