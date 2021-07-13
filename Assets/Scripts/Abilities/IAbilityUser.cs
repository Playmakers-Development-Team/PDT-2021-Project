using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    public interface IAbilityUser : ITenetBearer
    {
        public Vector2Int Coordinate { get; }

        void TakeAttack(int amount);
        
        void TakeDamage(int amount);

        void TakeKnockback(int amount);

        void TakeDefence(int amount);
    }
}
