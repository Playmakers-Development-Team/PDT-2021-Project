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
                CostType.Per => user.GetStacks(tenetType),
                CostType.Spend => user.GetStacks(tenetType),
                _ => 0
            };
        }

        public int CalculateValue(IUnit user, int modifier)
        {
            return costType switch
            {
                CostType.With => modifier,
                CostType.Per => user.GetStacks(tenetType),
                CostType.Spend => modifier,
                _ => 0
            };
        }

        public void Expend(IUnit user)
        {
            switch (costType)
            {
                case CostType.With:
                    user.Expend(tenetType, 0);
                    break;
                
                case CostType.Per:
                    user.Expend(tenetType, user.GetStacks(tenetType));
                    break;
                
                case CostType.Spend:
                    user.Expend(tenetType, user.GetStacks(tenetType));
                    break;
                
                default:
                    return;
            }
        }
    }
}
