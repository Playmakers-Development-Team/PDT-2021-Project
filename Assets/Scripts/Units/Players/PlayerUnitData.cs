using System;

namespace Units.Players
{
    [Serializable]
    public class PlayerUnitData : UnitData
    {
        public PlayerUnitData(PlayerUnitData data)
        {
            // TODO: The rest of the stats.
            Abilities = data.Abilities;
        }
    }
}