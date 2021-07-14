using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abilities.Costs
{
    /// <summary>
    /// A convenient class that stores all the different type of costs into one class.
    /// The Composite Pattern for a better structure and organization.
    /// </summary>
    [Serializable]
    public class CompositeCost : ICost
    {
        [SerializeField] protected TenetCost[] tenetCosts;
        
        // All cost variables should be put together here.
        private IEnumerable<ICost> AllChildCosts => tenetCosts;
        
        public void ApplyCost(IAbilityUser user, IAbilityUser target)
        {
            foreach (var childCost in AllChildCosts)
                childCost.ApplyCost(user, target);
        }

        public bool MeetsRequirements(IAbilityUser user, IAbilityUser target) =>
            AllChildCosts.All(c => c.MeetsRequirements(user, target));
    }
}
