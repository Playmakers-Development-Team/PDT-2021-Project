using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// Represents anything that could use an ability.
    /// </summary>
    public interface IAbilityUser : ITenetBearer
    {
        public Vector2Int Coordinate { get; }
        
        public string Name { get; }

        void AddSpeed(int amount);

        void TakeAttack(int amount);
        
        void TakeDamage(int amount);
        
        void DealDamageTo(IAbilityUser other, int amount);

        void TakeKnockback(int amount);

        void TakeDefence(int amount);

        void TakeAttackForEncounter(int amount);

        void TakeDefenceForEncounter(int amount);

        bool IsSameTeamWith(IAbilityUser other);

        IVirtualAbilityUser CreateVirtualAbilityUser();
    }
}
