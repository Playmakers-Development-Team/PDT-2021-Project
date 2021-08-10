using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Abilities.Parsing;
using Abilities.Shapes;
using UnityEngine;
using Utilities;

namespace Abilities.Costs
{
    [Serializable]
    public class ShapeCost : ICost
    {
        [SerializeField] private BasicShapeData shape;
        [SerializeField] private ShapeCountConstraint countConstraint;
        [SerializeField, Min(0)] private int count;
        [SerializeField] private ShapeFilter shapeFilter;
        [SerializeField] private CompositeCost cost;

        public IShape Shape => shape;

        public string DisplayName
        {
            get
            {
                string costString = cost.CostType switch
                {
                    CostType.None => string.Empty,
                    CostType.Shape => "~CAN'T HAVE SHAPE COST!",
                    _ => $" where {cost.DisplayName}"
                };
                string shapeName = shape != null ? shape.name : "no defined shape";
                string countConstraintString = StringUtility.UppercaseToReadable(countConstraint);
                string shapeFilterString = StringUtility.UppercaseToReadable(shapeFilter);
                
                return $"finds {countConstraintString} {count} {shapeFilterString} in {shapeName}{costString}";
            }
        }

        public void ApplyCost(IAbilityContext context, IAbilityUser unit)
        {
            // Keep in an array here to prevent potential modification exceptions
            IAbilityUser[] targets = countConstraint switch
            {
                // In case there are more units than the count, we want to randomise the order and
                // pick the count amount
                ShapeCountConstraint.AtLeast => GetShapeTargets(context, unit)
                    .OrderBy(left => UnityEngine.Random.Range(int.MinValue, int.MaxValue))
                    .Take(count)
                    .ToArray(),
                ShapeCountConstraint.AtMost => GetShapeTargets(context, unit).ToArray(),
                _ => throw new ArgumentOutOfRangeException(
                    $"Unsupported {nameof(ShapeCountConstraint)} {countConstraint}")
            };
            
            foreach (IAbilityUser target in targets)
                cost.ApplyAnyTargetCost(context, target);
            
            cost.ApplyAnyUserCost(context, unit);
        }

        public bool MeetsRequirements(IAbilityContext context, IAbilityUser user) =>
            countConstraint == ShapeCountConstraint.AtLeast
                ? GetShapeTargets(context, user).Count() >= count
                : GetShapeTargets(context, user).Count() <= count;

        private IEnumerable<IAbilityUser> GetShapeTargets(IAbilityContext context, IAbilityUser unit) =>
            cost.MeetsRequirementsForUser(context, unit)
                ? context.GetCachedUsersFromShape(unit.Coordinate, Vector2.zero, shape)
                    .Where(target => MatchesShapeFilter(unit, target))
                    .Where(target => cost.MeetsRequirementsForTarget(context, target))
                : Enumerable.Empty<IAbilityUser>();

        // TODO: Duplicate code, see ShapeBonus.MatchesShapeFilter
        private bool MatchesShapeFilter(IAbilityUser user, IAbilityUser target)
        {
            return shapeFilter switch
            {
                ShapeFilter.AnyTeam => true,
                ShapeFilter.SameTeam => user.IsSameTeamWith(target),
                ShapeFilter.OtherTeam => !user.IsSameTeamWith(target),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
