using GridObjects;
using UnityEngine;

namespace Units
{
    public class Unit : GridObject, IUnit
    {
        public Unit(
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        ) : base(position, dealDamageModifier, takeDamageModifier, takeKnockbackModifier) {}
    }
}
