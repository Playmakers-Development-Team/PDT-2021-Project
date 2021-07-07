using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Conditionals;
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
        [SerializeField] private List<Bonus> bonuses;
        [SerializeField] private List<Cost> costs;
        [SerializeField] private List<Keyword> keywords;

        private int TotalDamage => damageValue + AllKeywordEffects.Sum(e => e.damageValue);
        private int TotalDefence => defenceValue + AllKeywordEffects.Sum(e => e.defenceValue);
        private int TotalAttack => attackValue + AllKeywordEffects.Sum(e => e.attackValue);
        
        private IEnumerable<Effect> AllKeywordEffects => keywords.Select(k => k.Effect);
        private IEnumerable<Cost> AllCosts => AllKeywordEffects
            .SelectMany(e => e.costs)
            .Concat(costs);
        private IEnumerable<Bonus> AllBonuses => AllKeywordEffects
            .SelectMany(e => e.bonuses)
            .Concat(bonuses);

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
            return AllCosts.All(cost => cost.MeetsRequirements(user, target));
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
            foreach (Cost cost in AllCosts)
                cost.ApplyCost(user, target);
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

            int bonusSum = AllBonuses
                .Sum(bonus => bonus.CalculateBonusMultiplier(user, target));

            return value + bonusSum;
        }
    }
}
