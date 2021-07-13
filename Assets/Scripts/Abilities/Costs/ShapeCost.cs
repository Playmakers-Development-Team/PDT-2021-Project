using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Abilities.Shapes;
using Units;
using UnityEngine;

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
                
                return $"finds {UppercaseCaseToReadable(countConstraint)} {count} " 
                       + $"{UppercaseCaseToReadable(shapeFilter)} in {shapeName}{costString}";
            }
        }

        public void ApplyCost(IUnit unit)
        {
            // Keep in an array here to prevent potential modification exceptions
            IUnit[] targets = countConstraint switch
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
            
            foreach (IUnit target in targets)
                cost.ApplyCost(unit, target);
        }

        public bool MeetsRequirements(IUnit unit) =>
            countConstraint == ShapeCountConstraint.AtLeast
                ? GetShapeTargets(unit).Count() >= count
                : GetShapeTargets(unit).Count() <= count; 
        
        private IEnumerable<IUnit> GetShapeTargets(IUnit unit) => 
            shape.GetTargets(unit.Coordinate, Vector2.zero)
                .OfType<IUnit>()
                .Where(target => MatchesShapeFilter(unit, target))
                .Where(target => cost.MeetsRequirements(unit, target));

        private bool MatchesShapeFilter(IUnit unit, IUnit target) =>
            shapeFilter == ShapeFilter.AnyTeam || shapeFilter == ShapeFilter.SameTeam
                ? unit.IsSameTeamWith(target)
                : !unit.IsSameTeamWith(target);
        
        /// <summary>
        /// Put spaces between uppercase letters
        /// </summary> 
        private string UppercaseCaseToReadable(object o) =>
            Regex.Replace(o.ToString(), @"([A-Z])", " $0").Substring(1);
    }
}
