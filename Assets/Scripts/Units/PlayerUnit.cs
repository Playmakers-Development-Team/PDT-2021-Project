using Units;
using GridObjects;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit
    {
        public PlayerUnit(
        int healthPoints,
        int movementActionPoints,
        Vector2Int position, 
        Stat dealDamageModifier,
        Stat takeDamageModifier,
        Stat takeKnockbackModifier
        ) : base(healthPoints,movementActionPoints,position, dealDamageModifier, takeDamageModifier, 
        takeKnockbackModifier) {}
    }
}
  

