using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Bonuses;
using Abilities.Costs;
using Units;
using Units.TenetStatuses;
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
        [SerializeField] private CompositeCost cost;
        [SerializeField] private CompositeBonus bonus;
        [SerializeField] private List<Keyword> keywords;

        private int TotalDamage => damageValue + AllKeywordEffects.Sum(e => e.damageValue);
        private int TotalDefence => defenceValue + AllKeywordEffects.Sum(e => e.defenceValue);
        private int TotalAttack => attackValue + AllKeywordEffects.Sum(e => e.attackValue);
        
        private IEnumerable<Effect> AllKeywordEffects => keywords.Select(k => k.Effect);
        private IEnumerable<CompositeCost> AllCosts => AllKeywordEffects
            .Select(e => e.cost)
            .Prepend(cost);
        private IEnumerable<CompositeBonus> AllBonuses => AllKeywordEffects
            .Select(e => e.bonus)
            .Prepend(bonus);

        public IReadOnlyList<Keyword> Keywords => keywords.AsReadOnly();

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

            value *= AllBonuses.Sum(b => b.CalculateBonusMultiplier(user, target));

            return value;
        }
    }
}
