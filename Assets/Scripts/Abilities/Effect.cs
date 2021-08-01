using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Bonuses;
using Abilities.Costs;
using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Effect
    {
        [SerializeField, HideInInspector] private string name;
        // Public for now to keep allow changing for backward compat
        [SerializeField] public bool affectTargets = true;
        // Public for now to keep allow changing for backward compat
        [SerializeField] public bool affectUser;
        [Tooltip("Determine the order in which effects should be applied")]
        [SerializeField] private EffectOrder effectOrder;
        [SerializeField] private int damageValue;
        [Tooltip("Should the damage be applied without attack modifiers or anything else?")]
        [SerializeField] private bool directDamage;
        [Tooltip("Every Defence on a unit reduces damage received by 1")]
        [SerializeField] private int defenceValue;
        [Tooltip("Every Attack on a unit adds 1 extra damage")]
        [SerializeField] private int attackValue;
        [Tooltip("Defence that lasts the entire encounter")]
        [SerializeField] private int defenceForEncounter;
        [Tooltip("Attack that lasts the entire encounter")]
        [SerializeField] private int attackForEncounter;
        [SerializeField] private TenetStatus providingTenet;
        [SerializeField] private WholeBonus[] bonuses;
        [SerializeField] private WholeCost[] costs;
        [SerializeField] private List<Keyword> keywords;

        private int TotalDamage => damageValue + AllKeywordEffects.Sum(e => e.damageValue);
        private int TotalDefence => defenceValue + AllKeywordEffects.Sum(e => e.defenceValue);
        private int TotalAttack => attackValue + AllKeywordEffects.Sum(e => e.attackValue);
        private int TotalDefenceForEncounter =>
            defenceForEncounter + AllKeywordEffects.Sum(e => e.defenceForEncounter);
        private int TotalAttackForEncounter =>
            attackForEncounter + AllKeywordEffects.Sum(e => e.attackForEncounter);

        public IEnumerable<Effect> AllKeywordEffects => Keywords.Select(k => k.Effect);
        private IEnumerable<CompositeCost> KeywordsCosts => AllKeywordEffects
            .SelectMany(e => e.costs)
            .Distinct(); // We only do keyword costs once, so only need unique ones
        private IEnumerable<CompositeCost> AllCosts => KeywordsCosts.Concat(costs);
        private IEnumerable<CompositeBonus> AllBonuses => AllKeywordEffects
            .SelectMany(e => e.bonuses)
            .Concat(bonuses);

        public IEnumerable<Keyword> Keywords => keywords.Where(k => k != null);
        public EffectOrder EffectOrder => effectOrder;

        public bool CanBeUsedWith(IAbilityUser user, IAbilityUser target) =>
            AllCosts.All(c => c.MeetsRequirementsWith(user, target));

        public bool CanBeUsedByUser(IAbilityUser user) =>
            AllCosts.All(c => c.MeetsRequirementsForUser(user));
        
        public bool CanBeUsedForTarget(IAbilityUser target) =>
            AllCosts.All(c => c.MeetsRequirementsForTarget(target));

        /// <summary>
        /// Give the IAbilityUser any tenets that this effect gives.
        /// </summary>
        public void ProvideTenet(IAbilityUser user)
        {
            user.AddOrReplaceTenetStatus(providingTenet.TenetType, providingTenet.StackCount);

            foreach (Effect effect in AllKeywordEffects)
            {
                user.AddOrReplaceTenetStatus(effect.providingTenet.TenetType,
                    effect.providingTenet.StackCount);
            }
        }

        /// <summary>
        /// Apply costs both from effects and keywords.
        /// </summary>
        /// <param name="user">The IAbilityUser we are specifying</param>
        /// <param name="isTarget">True if the costs should consider the IAbilityUser as a target</param>
        public void ApplyCombinedCosts(IAbilityUser user, bool isTarget)
        {
            ApplyEffectCosts(user, isTarget);
            ApplyKeywordCosts(user, isTarget);
        }

        /// <summary>
        /// Apply all costs relating to this effect only. Does not include costs from keywords.
        /// </summary>
        /// <param name="user">The IAbilityUser we are specifying</param>
        /// <param name="isTarget">True if the costs should consider the IAbilityUser as a target</param>
        public void ApplyEffectCosts(IAbilityUser user, bool isTarget)
        {
            foreach (WholeCost cost in costs)
            {
                if (isTarget)
                    cost.ApplyAnyTargetCost(user);
                else
                    cost.ApplyAnyUserCost(user);
            }
        }
        /// <summary>
        /// Apply all costs from keywords.
        /// </summary>
        /// <param name="user">The IAbilityUser we are specifying</param>
        /// <param name="isTarget">True if the costs should consider the IAbilityUser as a target</param>
        public void ApplyKeywordCosts(IAbilityUser user, bool isTarget)
        {
            foreach (CompositeCost cost in KeywordsCosts)
            {
                if (isTarget)
                    cost.ApplyAnyTargetCost(user);
                else
                    cost.ApplyAnyUserCost(user);
            }
        }

        public int CalculateValue(IAbilityUser user, IAbilityUser target, EffectValueType valueType)
        {
            int value = valueType switch
            {
                EffectValueType.Damage => directDamage ? 0 : TotalDamage,
                EffectValueType.DirectDamage => directDamage ? TotalDamage : 0,
                EffectValueType.Defence => TotalDefence,
                EffectValueType.Attack => TotalAttack,
                EffectValueType.DefenceForEncounter => TotalDefenceForEncounter,
                EffectValueType.AttackForEncounter => TotalAttackForEncounter,
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };

            int bonusSum = AllBonuses.Sum(b => b.CalculateBonusMultiplier(user, target));
            value *= Mathf.Max(1, bonusSum);

            return value;
        }
    }
}
