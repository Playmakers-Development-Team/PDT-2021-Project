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
    public class CompositeCost : Conditional
    {
        [SerializeField] private CostType costType;

        [SerializeField] private TenetCost tenetCost;
        
        private ICost ChildCost => costType switch
        {
            CostType.Tenet => tenetCost,
            _ => throw new ArgumentOutOfRangeException($"Unsupported {nameof(CostType)} {costType}")
        };

        public void ApplyCost(IUnit user, IUnit target) =>
            ChildCost.ApplyCost(GetAffectedUnit(user, target));

        public bool MeetsRequirements(IUnit user, IUnit target) =>
            ChildCost.MeetsRequirements(GetAffectedUnit(user, target));
    }
}
