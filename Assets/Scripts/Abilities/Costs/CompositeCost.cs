using System;
using System.Collections.Generic;
using System.Linq;
using Units;
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

        public void ApplyCost(IUnit user, IUnit target)
        {
            if (costType == CostType.None)
                return;
            
            ChildCost.ApplyCost(GetAffectedUnit(user, target));
        }

        public bool MeetsRequirements(IUnit user, IUnit target) =>
            costType == CostType.None || ChildCost.MeetsRequirements(GetAffectedUnit(user, target));
    }
}
