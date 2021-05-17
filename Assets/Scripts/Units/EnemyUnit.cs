using GridObjects;
using UnityEngine;

namespace Units
{
    public class EnemyUnit : Unit
    {
        public EnemyUnit(
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        ) : base(position, dealDamageModifier, takeDamageModifier, takeKnockbackModifier) {}
    }
}
