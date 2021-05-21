using GridObjects;
using UnityEngine;

namespace Units
{
    public class EnemyUnit : Unit
    {
        public EnemyUnit(
            int speed,
            int healthPoints,
            int movementActionPoints,
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        ) : base(healthPoints,movementActionPoints,speed,position, dealDamageModifier, 
        takeDamageModifier, 
        takeKnockbackModifier) {}
    }
}
