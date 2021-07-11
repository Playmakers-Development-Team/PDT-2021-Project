using System;
using UnityEngine;

namespace Abilities.Costs
{
    /// <summary>
    /// A more complete version of <see cref="CompositeCost"/>.
    /// We use this class as the final cost class to prevent recursion errors in the editor.
    /// </summary>
    [Serializable]
    public class WholeCost : CompositeCost
    {
        [SerializeField] private ShapeCost shapeCost;

        protected override ICost ChildCost => costType switch 
        {
            CostType.Shape => shapeCost,
            _ => base.ChildCost
        };
    }
}
