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
        
        
        public bool CanUse(IUnit user)
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

        public void Provide(IUnit user) => 
            user.AddOrReplaceTenetStatusEffect(providingTenet.TenetType, providingTenet.StackCount);

        public void Expend(IUnit user)
        {
            if (!CanUse(user))
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
            
            if (costs.Count == 0)
                return value;
            
            int bonus = value;

            foreach (Cost cost in costs)
                bonus += cost.CalculateValue(user, value);
            
            return bonus;
        }
    }
}
