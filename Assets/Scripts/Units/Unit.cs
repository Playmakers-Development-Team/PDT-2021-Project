using GridObjects;
using UnityEngine;

namespace Units
{
    public class Unit : GridObject, IUnit
    {
        public Unit(
            Vector2Int position,
            ITakeDamageBehaviour takeDamageBehaviour,
            ITakeKnockbackBehaviour takeKnockbackBehaviour
        ) : base(position, takeDamageBehaviour, takeKnockbackBehaviour) {}
    }
}
