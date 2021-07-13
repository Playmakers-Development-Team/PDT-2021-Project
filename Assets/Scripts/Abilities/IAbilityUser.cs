using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    public interface IAbilityUser : ITenetBearer
    {
        public Vector2Int Coordinate { get; }

        void TakeAttack(int amount);
        
        void TakeDamageWithoutModifiers(int amount);
        
        void DealDamageTo(IAbilityUser other, int amount);

        void TakeKnockback(int amount);

        void TakeDefence(int amount);
        
        bool IsSameTeamWith(IAbilityUser other);
    }
}
