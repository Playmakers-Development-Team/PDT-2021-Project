using GridObjects;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit
    {
        public PlayerUnit(
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        ) : base(position, dealDamageModifier, takeDamageModifier, takeKnockbackModifier) {}
    }
}
