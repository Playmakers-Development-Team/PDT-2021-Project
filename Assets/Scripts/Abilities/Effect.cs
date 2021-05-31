using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private TenetStatusEffect providingTenet;
        [SerializeField] private List<Cost> costs;

        public void Use(IUnit user)
        {
            if (CanBeUsedBy(user))
            {
                Provide(user);
                Expend(user);
            }
        }
        
        public bool CanBeUsedBy(IUnit user)
        {
            return costs.All(cost => cost.MeetsRequirements(user));
        }

        private void Provide(IUnit user) => 
            user.AddOrReplaceTenetStatusEffect(providingTenet.TenetType, providingTenet.StackCount);

        private void Expend(IUnit user)
        {
            if (!CanBeUsedBy(user))
                return;

            foreach (Cost cost in costs)
                cost.Expend(user);
        }

        // TODO: Test
        public int CalculateValue(IUnit user, EffectValueType valueType)
        {
            int value = valueType switch
            {
                EffectValueType.Damage => damageValue,
                EffectValueType.Defence => defenceValue,
                EffectValueType.Attack => attackValue,
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };

            int bonusValue = 0;
            
            foreach (Cost cost in costs)
                bonusValue += cost.CalculateBonusValue(user);
            
            return bonusValue == 0 ? value : bonusValue * value;
        }
    }
}
