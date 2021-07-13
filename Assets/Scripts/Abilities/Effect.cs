using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Bonuses;
using Abilities.Costs;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Effect
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private int damageValue;
        [SerializeField] private int defenceValue;
        [SerializeField] private int attackValue;
        [SerializeField] private TenetStatus providingTenet;
        [SerializeField] private CompositeBonus[] bonuses;
        [SerializeField] private WholeCost[] costs;
        [SerializeField] private List<Keyword> keywords;

        private int TotalDamage => damageValue + AllKeywordEffects.Sum(e => e.damageValue);
        private int TotalDefence => defenceValue + AllKeywordEffects.Sum(e => e.defenceValue);
        private int TotalAttack => attackValue + AllKeywordEffects.Sum(e => e.attackValue);
        
        private IEnumerable<Effect> AllKeywordEffects => Keywords.Select(k => k.Effect);
        private IEnumerable<CompositeCost> AllCosts => AllKeywordEffects
            .SelectMany(e => e.costs)
            .Concat(costs);
        private IEnumerable<CompositeBonus> AllBonuses => AllKeywordEffects
            .SelectMany(e => e.bonuses)
            .Concat(bonuses);

        public IEnumerable<Keyword> Keywords => keywords.Where(k => k != null);

        public bool ProcessTenet(IUnit user, IUnit target)
        {
            if (CanBeUsedBy(user, target))
            {
                ProvideTenet(target);
                ApplyCosts(user, target);
                return true;
            }

            return false;
        }
        
        public bool CanBeUsedBy(IUnit user, IUnit target)
        {
            return AllCosts.All(c => c.MeetsRequirements(user, target));
        }

        private void ProvideTenet(IUnit unit)
        {
            unit.AddOrReplaceTenetStatus(providingTenet.TenetType, providingTenet.StackCount);

            foreach (Effect effect in AllKeywordEffects)
            {
                unit.AddOrReplaceTenetStatus(effect.providingTenet.TenetType,
                    effect.providingTenet.StackCount);
            }
        }

        private void ApplyCosts(IUnit user, IUnit target)
        {
            foreach (CompositeCost compositeCost in AllCosts)
                compositeCost.ApplyCost(user, target);
        }

        public int CalculateValue(IUnit user, IUnit target, EffectValueType valueType)
        {
            int value = valueType switch
            {
                EffectValueType.Damage => TotalDamage,
                EffectValueType.Defence => TotalDefence,
                EffectValueType.Attack => TotalAttack,
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };

            int bonusSum = AllBonuses.Sum(b => b.CalculateBonusMultiplier(user, target));
            value *= Mathf.Max(1, bonusSum);

            return value;
        }
    }
}
