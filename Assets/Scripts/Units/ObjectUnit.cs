using GridObjects;
using UnityEngine;

namespace Units
{
    public class ObjectUnit : Unit
    {
        public ObjectUnit(
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
            //IUnit consisting of type of obstacle? (Barricade, Slow, Hazar)
        ) : base(position, dealDamageModifier, takeDamageModifier, takeKnockbackModifier /*, Obstacle Type? */) {}
    }
}