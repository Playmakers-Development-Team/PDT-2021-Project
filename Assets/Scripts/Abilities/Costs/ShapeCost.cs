using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Shapes;
using Units;
using UnityEngine;

namespace Abilities.Costs
{
    public enum ShapeFilter
    {
        SameTeam, 
        OtherTeam
    }

    [Serializable]
    public class ShapeCost : ICost, ISerializationCallbackReceiver
    {
        [SerializeField] private BasicShapeData shape;
        [SerializeField, Min(1)] private int minCount = 1;
        [SerializeField] private ShapeFilter shapeFilter;
        [SerializeField] private CompositeCost cost;

        public IShape Shape => shape;

        public void ApplyCost(IUnit unit)
        {
            // In case there are more units than the minCount, we want to randomise the order 
            // Finally convert to array here to prevent potential modification exceptions
            var targets = GetShapeTargets(unit)
                .OrderBy((left) => UnityEngine.Random.Range(int.MinValue, int.MaxValue))
                .Take(minCount)
                .ToArray();
            
            foreach (IUnit target in targets)
                cost.ApplyCost(unit, target);
        }

        public bool MeetsRequirements(IUnit unit) => GetShapeTargets(unit).Count() >= minCount;
        
        private IEnumerable<IUnit> GetShapeTargets(IUnit unit) => 
            shape.GetTargets(unit.Coordinate, Vector2.zero).OfType<IUnit>()
                .Where(target => MatchesShapeFilter(unit, target))
                .Where(target => cost.MeetsRequirements(unit, target));

        private bool MatchesShapeFilter(IUnit unit, IUnit target) =>
            (shapeFilter == ShapeFilter.SameTeam && unit.IsSameTeamWith(target)) 
            || !unit.IsSameTeamWith(target);

        public void OnBeforeSerialize()
        {
            // We need this to circumvent unity issue that does not set default values
            minCount = Mathf.Max(1, minCount);
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
