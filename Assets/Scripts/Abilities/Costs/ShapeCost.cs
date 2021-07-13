using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Abilities.Shapes;
using UnityEngine;
using Utilities;

namespace Abilities.Costs
{
    public enum ShapeFilter
    {
        AnyTeam,
        SameTeam, 
        OtherTeam
    }

    public enum ShapeCountConstraint
    {
        AtLeast,
        AtMost,
    }

    [Serializable]
    public class ShapeCost : ICost
    {
        [SerializeField] private BasicShapeData shape;
        [SerializeField, Min(0)] private int count;
        [SerializeField] private ShapeCountConstraint countConstraint;
        [SerializeField] private ShapeFilter shapeFilter;
        [SerializeField] private CompositeCost cost;

        public IShape Shape => shape;

        public string DisplayName
        {
            get
            {
                string costString = cost.CostType != CostType.None ? $" where {cost}" : string.Empty;
                string shapeName = shape != null ? shape.name : "no defined shape";
                string countConstraintString = StringUtility.UppercaseToReadable(countConstraint);
                string shapeFilterString = StringUtility.UppercaseToReadable(shapeFilter);
                
                return $"finds {countConstraintString} {count} {shapeFilterString} in {shapeName}{costString}";
            }
        }

        public void ApplyCost(IAbilityUser unit)
        {
            // Keep in an array here to prevent potential modification exceptions
            IAbilityUser[] targets = countConstraint switch
            {
                // In case there are more units than the count, we want to randomise the order and
                // pick the count amount
                ShapeCountConstraint.AtLeast => GetShapeTargets(unit)
                    .OrderBy((left) => UnityEngine.Random.Range(int.MinValue, int.MaxValue))
                    .Take(count)
                    .ToArray(),
                ShapeCountConstraint.AtMost => GetShapeTargets(unit).ToArray(),
                _ => throw new ArgumentOutOfRangeException(
                    $"Unsupported {nameof(ShapeCountConstraint)} {countConstraint}")
            };
            
            foreach (IAbilityUser target in targets)
                cost.ApplyCost(unit, target);
        }

        public bool MeetsRequirements(IAbilityUser unit) =>
            countConstraint == ShapeCountConstraint.AtLeast
                ? GetShapeTargets(unit).Count() >= count
                : GetShapeTargets(unit).Count() <= count; 
        
        private IEnumerable<IAbilityUser> GetShapeTargets(IAbilityUser unit) => 
            shape.GetTargets(unit.Coordinate, Vector2.zero)
                .OfType<IAbilityUser>()
                .Where(target => MatchesShapeFilter(unit, target))
                .Where(target => cost.MeetsRequirements(unit, target));

        private bool MatchesShapeFilter(IAbilityUser unit, IAbilityUser target) =>
            shapeFilter == ShapeFilter.AnyTeam || shapeFilter == ShapeFilter.SameTeam
                ? unit.IsSameTeamWith(target)
                : !unit.IsSameTeamWith(target);
    }
}
