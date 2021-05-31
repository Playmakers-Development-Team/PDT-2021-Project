using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private CostType costType;
        [SerializeField] private TenetType tenetType;

        
        public TenetType TenetType => tenetType;
        

        public int CalculateCost(IUnit user)
        {
            return costType switch
            {
                CostType.With => 1,
                CostType.Per => user.GetTenetStatusEffectCount(tenetType),
                CostType.Spend => user.GetTenetStatusEffectCount(tenetType),
                _ => 0
            };
        }

        public int CalculateValue(IUnit user, int modifier)
        {
            return costType switch
            {
                CostType.With => modifier,
                CostType.Per => user.GetTenetStatusEffectCount(tenetType),
                CostType.Spend => modifier,
                _ => 0
            };
        }

        public void Expend(IUnit user)
        {
            user.RemoveTenetStatusEffect(tenetType, CalculateCost(user));
        }
    }
}
