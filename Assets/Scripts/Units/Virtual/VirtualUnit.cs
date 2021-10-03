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
    /// which in theory will save performance and memory. This is assuming that it this object
    /// is created and destroyed often without changing most of the stats.</p>
    /// </summary>
    public class VirtualUnit : IVirtualAbilityUser
    {
        public IUnit Unit { get; }
        public IAbilityUser RealAbilityUser => Unit;
        public Vector2Int Coordinate => Unit.Coordinate;
        public string Name
        {
            get => Unit.Name;
            set => Unit.Name = value;
        }

        private VirtualStat attack;
        private VirtualStat defence;
        private VirtualStat health;
        private VirtualStat knockback;
        private VirtualStat speed;
        private VirtualStat movementPoints;
        private ITenetBearer tenetBearer;
        private bool isDealingDamage;

        // All the stats are Lazy, so that we save on memory and performance when processing/parsing
        public VirtualStat Attack => attack ??= new VirtualStat(Unit.AttackStat);
        public VirtualStat Defence => defence ??= new VirtualStat(Unit.DefenceStat);
        public VirtualStat Health => health ??= new VirtualStat(Unit.HealthStat, (current, delta) => 
            delta < 0 
                ? current + Mathf.Min(0, delta + Defence.TotalValue)
                : current + delta
            );
        public VirtualStat Knockback => knockback ??= new VirtualStat(Unit.KnockbackStat);
        public VirtualStat Speed => speed ??= new VirtualStat(Unit.SpeedStat);
        public VirtualStat MovementPoints => movementPoints ??= new VirtualStat(Unit.MovementPoints);
        public ITenetBearer TenetBearer => tenetBearer ??= new TenetStatusEffectsContainer(Unit.TenetStatuses);

        internal VirtualUnit(IUnit unit) => Unit = unit;

        public void TakeDefence(int amount) => Defence.ValueDelta += amount;

        public void TakeAttack(int amount) => Attack.ValueDelta += amount;

        public void TakeAttackForEncounter(int amount) => Attack.BaseValueDelta += amount;

        public void TakeDefenceForEncounter(int amount) => Defence.BaseValueDelta += amount;

        public void TakeDamage(int amount)
        {
            if (amount > 0)
                Health.ValueDelta -= amount;
        }

        public void DealDamageTo(IAbilityUser other, int amount)
        {
            // Attack modifiers should only be applied when damage amount is non-zero
            if (amount > 0)
            {
                int damage = amount + Attack.TotalValue;
                other.TakeDamage(damage);
                isDealingDamage = true;
            }
        }

        public void TakeKnockback(int amount) => Knockback.ValueDelta += amount;

        public void AddSpeed(int amount) => Speed.ValueDelta += amount;

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
            
            if (health != null && Health.ValueDelta < 0)
                Unit.HealthStat.TakeDamage(-Health.ValueDelta);
            
            // Reset attack after dealing damage
            if (attack != null && isDealingDamage)
                Attack.ResetValues();

            if (tenetBearer != null)
                Unit.SetTenets(TenetBearer);
        }
    }
}
