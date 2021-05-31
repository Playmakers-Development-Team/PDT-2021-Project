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
                CostType.Spend => 1,
                _ => 0
            };
        }

        public int CalculateValue(IUnit user, int baseValue)
        {
            return costType switch
            {
                CostType.With => baseValue,
                CostType.Per => baseValue * user.GetTenetStatusEffectCount(tenetType),
                CostType.Spend => baseValue,
                _ => 0
            };
        }

        public void Expend(IUnit user)
        {
            switch (costType)
            {
                case CostType.Per:
                    user.RemoveTenetStatusEffect(tenetType, user.GetTenetStatusEffectCount(tenetType));
                    break;
                
                case CostType.Spend:
                    user.RemoveTenetStatusEffect(tenetType, 1);
                    break;
            }
        }
    }
}
