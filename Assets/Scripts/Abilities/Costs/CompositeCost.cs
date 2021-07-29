using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abilities.Costs
{
    /// <summary>
    /// Uses the Composite Pattern to store all the different types of costs in one class.
    /// </summary>
    [Serializable]
    public class CompositeCost : Conditional, IDisplayable
    {
        [SerializeField] protected CostType costType;
        [CompositeChild((int) CostType.Tenet)]
        [SerializeField] protected TenetCost tenetCost;
        [CompositeChild((int) CostType.TenetGroup)]
        [SerializeField] protected TenetGroupCost tenetGroupCost;
        // Put more types of costs here, they need to be protected to be read by the property drawer

        public CostType CostType => costType;
        
        public string DisplayName => ChildCost.DisplayName;
        
        protected virtual ICost ChildCost => costType switch
        {
            CostType.None => throw new ArgumentException(
                $"Trying to get child cost but {nameof(CostType)} has not been set!"),
            CostType.Tenet => tenetCost,
            CostType.TenetGroup => tenetGroupCost,
            CostType.Shape => throw new ArgumentException(
                $"Cannot have a recursive costs, E.g shape cost inside a shape cost!"),
            _ => throw new ArgumentOutOfRangeException(
                $"Unsupported {nameof(CostType)} {costType} for {nameof(CompositeCost)}")
        };

        public void ApplyAnyTargetCost(IAbilityUser target)
        {
            if (affectType == AffectType.Target && costType != CostType.None)
                ChildCost.ApplyCost(target);
        }

        public void ApplyAnyUserCost(IAbilityUser user)
        {
            if (affectType == AffectType.User && costType != CostType.None)
                ChildCost.ApplyCost(user);
        }
        
        public bool MeetsRequirementsForUser(IAbilityUser user) =>
            costType == CostType.None || affectType == AffectType.User && ChildCost.MeetsRequirements(user);

        public bool MeetsRequirementsForTarget(IAbilityUser target) =>
            costType == CostType.None || affectType == AffectType.Target && ChildCost.MeetsRequirements(target);

        public bool MeetsRequirementsWith(IAbilityUser user, IAbilityUser target) =>
            costType == CostType.None || ChildCost.MeetsRequirements(GetAffectedUser(user, target));
    }
}
