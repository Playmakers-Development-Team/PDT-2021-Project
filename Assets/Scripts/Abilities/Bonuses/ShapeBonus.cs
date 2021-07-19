using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Shapes;
using UnityEngine;
using Utilities;

namespace Abilities.Bonuses
{
    [Serializable]
    public class ShapeBonus : IBonus
    {
        [SerializeField] private BasicShapeData shape;
        [SerializeField] private ShapeCountConstraint countConstraint;
        [SerializeField] private int count;
        [SerializeField] private ShapeFilter shapeFilter;
        [SerializeField] private bool bonusByCount = true;
        [SerializeField] private CompositeBonus bonus;

        public string DisplayName
        {
            get
            {
                string shapeString = shape ? shape.name : "No defined shape";
                string countConstraintString = StringUtility.UppercaseToReadable(countConstraint);
                string shapeFilterString = StringUtility.UppercaseToReadable(shapeFilter);
                string shapeBonusCountString = bonusByCount ? $" counted" : string.Empty;
                string extraBonusString = bonus.BonusType switch
                {
                    BonusType.None => string.Empty,
                    BonusType.Shape => "~CAN'T HAVE SHAPE BONUS!",
                    _ => $" with {bonus.DisplayName}" 
                };
                
                return $"{countConstraintString} {count} {shapeFilterString} in {shapeString}"
                       + $"{shapeBonusCountString}{extraBonusString}";
            }
        }

        public int CalculateBonusMultiplier(IAbilityUser user)
        {
            if (shape == null)
                return 0;

            var targets = GetValidShapeTargets(user).ToArray();

            int baseBonus = bonusByCount ? targets.Length : 0;
            int childBonus = targets.Length > 0
                ? targets.Sum(u => bonus.CalculateBonusMultiplier(user, u))
                : 0;

            return baseBonus + childBonus;
        }

        private IEnumerable<IAbilityUser> GetValidShapeTargets(IAbilityUser user)
        {
            var targets = shape
                .GetTargets(user.Coordinate, Vector2.zero)
                .OfType<IAbilityUser>()
                .Where(u => MatchesShapeFilter(user, u))
                .ToArray();

            return countConstraint switch
            {
                // If we more than we can take, try to randomise and pick the maximum set amount
                ShapeCountConstraint.AtMost => targets
                    .OrderBy((left) => UnityEngine.Random.Range(int.MinValue, int.MaxValue))
                    .Take(count),
                ShapeCountConstraint.AtLeast => targets.Length >= count ? targets : Enumerable.Empty<IAbilityUser>(),
                _ => throw new ArgumentOutOfRangeException(nameof(countConstraint), countConstraint, null)
            };
        }

        private bool MatchesShapeFilter(IAbilityUser user, IAbilityUser target) =>
            shapeFilter == ShapeFilter.AnyTeam || shapeFilter == ShapeFilter.SameTeam
                ? user.IsSameTeamWith(target)
                : !user.IsSameTeamWith(target);
    }
}
