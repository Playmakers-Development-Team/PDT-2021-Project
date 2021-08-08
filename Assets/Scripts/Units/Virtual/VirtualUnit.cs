using System;
using System.Collections.Generic;
using Abilities;
using TenetStatuses;
using Units.Stats;
using UnityEngine;

namespace Units.Virtual
{
    /// <summary>
    /// <p>Takes the stats and tenets of a unit and provide an easy way to modify them.
    /// When finally satisfied with modification, it can set them in one single step</p>
    ///
    /// <p>This class utilises the builder pattern for convenience.
    /// <see href="https://www.geeksforgeeks.org/builder-design-pattern/"/></p>
    /// <p>Also utilises Lazy pattern for the stats so that we save on heap space allocation
    /// which in theory will save performance and memory.</p>
    /// </summary>
    public class VirtualUnit : IVirtualAbilityUser
    {
        public IUnit Unit { get; }
        public IAbilityUser RealAbilityUser => Unit;
        public Vector2Int Coordinate => Unit.Coordinate;
        public string Name => Unit.Name;

        private VirtualStat attack;
        private VirtualStat defence;
        private VirtualStat health;
        private VirtualStat knockback;
        private VirtualStat speed;
        private VirtualStat movementPoints;
        private ITenetBearer tenetBearer;

        // All the stats are Lazy, so that we save on memory and performance
        private VirtualStat Attack => attack ??= new VirtualStat(Unit.AttackStat);
        private VirtualStat Defence => defence ??= new VirtualStat(Unit.DefenceStat);
        private VirtualStat Health => health ??= new VirtualStat(Unit.HealthStat);
        private VirtualStat Knockback => knockback ??= new VirtualStat(Unit.KnockbackStat);
        private VirtualStat Speed => speed ??= new VirtualStat(Unit.SpeedStat);
        private VirtualStat MovementPoints => movementPoints ??= new VirtualStat(Unit.MovementPoints);
        public ITenetBearer TenetBearer => tenetBearer ??= new TenetStatusEffectsContainer(Unit.TenetStatuses);

        internal VirtualUnit(IUnit unit) => Unit = unit;
        
        public void TakeDefence(int amount) => Defence.ValueOffset += amount;

        public void TakeAttack(int amount) => Attack.ValueOffset += amount;

        public void TakeAttackForEncounter(int amount) => Attack.BaseValueOffset += amount;

        public void TakeDefenceForEncounter(int amount) => Defence.BaseValueOffset += amount;

        public void TakeDamage(int amount)
        {
            if (amount > 0)
                Health.ValueOffset -= amount;
        }

        public void DealDamageTo(IAbilityUser other, int amount)
        {
            // Attack modifiers should only be applied when damage amount is non-zero
            if (amount > 0)
            {
                int damage = amount + Attack.TotalValue;
                other.TakeDamage(damage);
            }
        }

        public void TakeKnockback(int amount) => Knockback.ValueOffset += amount;

        public void AddSpeed(int amount) => Speed.ValueOffset += amount;

        public bool IsSameTeamWith(IAbilityUser other) =>
            other is IVirtualAbilityUser virtualAbilityUser
                ? Unit.IsSameTeamWith(virtualAbilityUser.RealAbilityUser)
                : Unit.IsSameTeamWith(other);

        public IVirtualAbilityUser CreateVirtualAbilityUser() => Unit.CreateVirtualAbilityUser();

        #region TenetStatuses

        public ICollection<TenetStatus> TenetStatuses => TenetBearer.TenetStatuses;

        public void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1) =>
            TenetBearer.AddOrReplaceTenetStatus(tenetType, stackCount);

        public bool RemoveTenetStatus(TenetType tenetType, int amount = Int32.MaxValue) =>
            TenetBearer.RemoveTenetStatus(tenetType, amount);

        public void ClearAllTenetStatus() => TenetBearer.ClearAllTenetStatus();

        public int GetTenetStatusCount(TenetType tenetType) => TenetBearer.GetTenetStatusCount(tenetType);

        public bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1) =>
            TenetBearer.HasTenetStatus(tenetType, minimumStackCount);

        public bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus) =>
            TenetBearer.TryGetTenetStatus(tenetType, out tenetStatus);

        public void SetTenets(ITenetBearer tenetBearer) => 
            TenetBearer.SetTenets(tenetBearer);
        
        #endregion

        public void ApplyChanges()
        {
            // Order matters here, modifiers like defence and attack should be applied first
            
            if (attack != null)
                Attack.SetValues();
            
            if (defence != null)
                Defence.SetValues();
            
            if (knockback != null)
                Knockback.SetValues();
            
            if (speed != null)
                Speed.SetValues();
            
            if (movementPoints != null)
                MovementPoints.SetValues();
            
            if (health != null)
                Unit.HealthStat.TakeDamage(-Health.ValueOffset);
            
            if (tenetBearer != null)
                Unit.SetTenets(TenetBearer);
        }
    }
}
