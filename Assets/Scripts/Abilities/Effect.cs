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

        private IEnumerable<Effect> AllKeywordEffects => Keywords.Select(k => k.Effect);
        private IEnumerable<CompositeCost> AllCosts => AllKeywordEffects
            .SelectMany(e => e.costs)
            .Concat(costs);
        private IEnumerable<CompositeBonus> AllBonuses => AllKeywordEffects
            .SelectMany(e => e.bonuses)
            .Concat(bonuses);

        public IEnumerable<Keyword> Keywords => keywords.Where(k => k != null);
        public EffectOrder EffectOrder => effectOrder;

        public bool ProcessTenet(IAbilityUser user, IAbilityUser target)
        {
            if (CanBeUsedBy(user, target))
            {
                ProvideTenet(target);
                ApplyCosts(user, target);
                return true;
            }

            return false;
        }
        
        public bool CanBeUsedBy(IAbilityUser user, IAbilityUser target)
        {
            return AllCosts.All(c => c.MeetsRequirements(user, target));
        }

        private void ProvideTenet(IAbilityUser unit)
        {
            unit.AddOrReplaceTenetStatus(providingTenet.TenetType, providingTenet.StackCount);

            foreach (Effect effect in AllKeywordEffects)
            {
                unit.AddOrReplaceTenetStatus(effect.providingTenet.TenetType,
                    effect.providingTenet.StackCount);
            }
        }

        private void ApplyCosts(IAbilityUser user, IAbilityUser target)
        {
            foreach (CompositeCost compositeCost in AllCosts)
                compositeCost.ApplyCost(user, target);
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
