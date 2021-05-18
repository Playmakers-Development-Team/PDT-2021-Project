using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private CostType costType;
        [SerializeField] private Tenet tenet;

        
        public Tenet Tenet => tenet;
        

        public int CalculateCost(IUnit user)
        {
            return costType switch
            {
                CostType.With => 1,
                CostType.Per => user.GetStacks(tenet),
                CostType.Spend => user.GetStacks(tenet),
                _ => 0
            };
        }

        public int CalculateValue(IUnit user, int modifier)
        {
            return costType switch
            {
                CostType.With => modifier,
                CostType.Per => user.GetStacks(tenet),
                CostType.Spend => modifier,
                _ => 0
            };
        }

        public void Expend(IUnit user)
        {
            switch (costType)
            {
                case CostType.With:
                    user.Expend(tenet, 0);
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
