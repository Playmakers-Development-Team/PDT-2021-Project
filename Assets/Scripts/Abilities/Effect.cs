using System;
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
        [SerializeField] private Cost[] costs;
        
        
        public bool CanUse(IUnit user)
        {
            int[] totalCosts = new int[Enum.GetValues(typeof(TenetType)).Length];

            foreach (Cost cost in costs)
                totalCosts[(int) cost.TenetType] += cost.CalculateCost(user);

            for (int i = 0; i < totalCosts.Length; i++)
            {
                if (user.GetStacks((TenetType) i) < totalCosts[i])
                    return false;
            }

            return true;
        }

        public void Expend(IUnit user)
        {
            if (!CanUse(user))
                return;

            foreach (Cost cost in costs)
                cost.Expend(user);
        }

        public int CalculateModifier(IUnit user, bool isDamage)
        {
            int bonus = costs.Length == 0
                ? isDamage ? damageValue : defenceValue
                : 0;
            
            foreach (Cost cost in costs)
                bonus += cost.CalculateValue(user, isDamage ? damageValue : defenceValue);
            
            return bonus;
        }
    }
}
