using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abilities.Costs
{
    /// <summary>
    /// A convenient class that stores all the different type of costs into one class.
    /// Sort of an implementation of the Composite Pattern for a better structure and organization.
    /// </summary>
    [Serializable]
    public class CompositeCost : Conditional, IDisplayable
    {
        [SerializeField] protected CostType costType;
        [CompositeChild((int) CostType.Tenet)]
        [SerializeField] protected TenetCost tenetCost;
        // Put more types of costs here, they need to be protected to be read by the property drawer

        public CostType CostType => costType;
        
        public string DisplayName => ChildCost.DisplayName;
        
        protected virtual ICost ChildCost => costType switch
        {
            CostType.None => throw new ArgumentException(
                $"Trying to get child cost but {nameof(CostType)} has not been set!"),
            CostType.Tenet => tenetCost,
            _ => throw new ArgumentOutOfRangeException(
                $"Unsupported {nameof(CostType)} {costType} for {nameof(CompositeCost)}")
        };

        public void ApplyCost(IAbilityUser user, IAbilityUser target)
        {
            if (costType == CostType.None)
                return;
            
            ChildCost.ApplyCost(GetAffectedUser(user, target));
        }

        public bool MeetsRequirements(IAbilityUser user, IAbilityUser target) =>
            costType == CostType.None || ChildCost.MeetsRequirements(GetAffectedUser(user, target));
    }
}
