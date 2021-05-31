using System;
using System.Collections.Generic;
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
            int[] totalCosts = new int[Enum.GetValues(typeof(TenetType)).Length];

            foreach (Cost cost in costs)
                totalCosts[(int) cost.TenetType] += cost.CalculateCost(user);

            for (int i = 0; i < totalCosts.Length; i++)
            {
                if (user.GetTenetStatusEffectCount((TenetType) i) < totalCosts[i])
                    return false;
            }

            return true;
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

        public int CalculateModifier(IUnit user, EffectValueType valueType)
        {
            int value = valueType switch
            {
                EffectValueType.Damage => damageValue,
                EffectValueType.Defence => defenceValue,
                EffectValueType.Attack => attackValue,
                _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
            };
            
            int bonus = costs.Count == 0 ? value : 0;
            
            foreach (Cost cost in costs)
                bonus += cost.CalculateValue(user, value);
            
            return bonus;
        }
    }
}
