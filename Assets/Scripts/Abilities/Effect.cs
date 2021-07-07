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

        public bool ProcessTenet(IUnit user, IUnit target)
        {
            if (CanBeUsedBy(user, target))
            {
                ProvideTenet(target);
                ApplyCost(user, target);
                return true;
            }

            return false;
        }
        
        public bool CanBeUsedBy(IUnit user, IUnit target)
        {
            return costs.All(cost => cost.MeetsRequirements(user, target));
        }

        private void ProvideTenet(IUnit unit) => 
            unit.AddOrReplaceTenetStatus(providingTenet.TenetType, providingTenet.StackCount);

        private void ApplyCost(IUnit user, IUnit target)
        {
            foreach (Cost cost in costs)
                cost.ApplyCost(user, target);
        }

        public int CalculateValue(IUnit user, IUnit target, EffectValueType valueType)
        {
            int value = valueType switch
            {
                EffectValueType.Damage => damageValue,
                EffectValueType.Defence => defenceValue,
                EffectValueType.Attack => attackValue,
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };

            int bonusMultiplierSum = bonuses.Sum(bonus => bonus.CalculateBonusMultiplier(user, target));

            return bonusMultiplierSum == 0 ? value : bonusMultiplierSum * value;
        }
    }
}
