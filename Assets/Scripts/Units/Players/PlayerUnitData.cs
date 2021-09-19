using System;

namespace Units.Players
{
    [Serializable]
    public class PlayerUnitData : UnitData
    {
        public PlayerUnitData(PlayerUnitData data)
        {
            Name = data.Name;
            HealthValue = data.HealthValue;
            Abilities = data.Abilities;
        }
    }
}