using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField] private CostType costType;
        [SerializeField] private Tenet tenet;


        public int CalculateCost(UnitData user)
        {
            return costType switch
            {
                CostType.With => 1,
                CostType.Per => user.GetStacks(tenet),
                CostType.Spend => user.GetStacks(tenet),
                _ => 0
            };
        }

        public int CalculateValue(UnitData user)
        {
            return costType switch
            {
                CostType.With => 1,
                CostType.Per => user.GetStacks(tenet),
                CostType.Spend => 1,
                _ => 0
            };
        }
        
        public bool CanAfford(UnitData user) => user.GetStacks(tenet) >= 1;

        public void Expend(UnitData user)
        {
            switch (costType)
            {
                case CostType.With:
                    user.Expend(tenet, 1);
                    break;
                
                case CostType.Per:
                    user.Expend(tenet, user.GetStacks(tenet));
                    break;
                
                case CostType.Spend:
                    user.Expend(tenet, user.GetStacks(tenet));
                    break;
                
                default:
                    return;
            }
        }
    }
}
