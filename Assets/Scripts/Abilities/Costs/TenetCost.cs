using System;
using TenetStatuses;
using UnityEngine;

namespace Abilities.Costs
{
    [Serializable]
    public enum TenetCostType
    {
        With, Consume, Spend
    }
    
    [Serializable]
    public class TenetCost : Conditional, ICost
    {
        [SerializeField] private TenetCostType tenetCostType;
        [SerializeField] private TenetType tenetType;
        
        public TenetCostType TenetCostType => tenetCostType;
        public TenetType TenetType => tenetType;
        
        public void ApplyCost(IAbilityUser user, IAbilityUser target)
        {
            IAbilityUser unit = GetAffectedUser(user, target);

            switch (TenetCostType)
            {
                case TenetCostType.Consume:
                    unit.RemoveTenetStatus(TenetType);
                    break;
                case TenetCostType.Spend:
                    unit.RemoveTenetStatus(TenetType, 1);
                    break;
            }
        }
        
        public bool MeetsRequirements(IAbilityUser user, IAbilityUser target) => 
            GetAffectedUser(user, target).GetTenetStatusCount(tenetType) > 0;
    }
}
