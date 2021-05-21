using System;

namespace Units
{
    [Serializable]
    public class PlayerUnitData : UnitData
    {
        public int CurrentHealth { get => CurrentHealth; set => CurrentHealth = value; }    
        private int currentMovementActionPoints;
        
        
    }
}
