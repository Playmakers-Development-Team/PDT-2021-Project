using System;
using TenetStatuses;
using UnityEngine;
using Utilities;

namespace Abilities.Bonuses
{
    [Serializable]
    public class TenetBonus : IBonus
    {
        [SerializeField] private TenetType tenetType;
        [SerializeField] private TenetConstraint tenetConstraint;

        public string DisplayName
        {
            get
            {
                string constraintString = tenetConstraint == TenetConstraint.None
                    ? string.Empty
                    : $" {StringUtility.UppercaseToReadable(tenetConstraint)}";
                return $"{tenetType}{constraintString}";
            }
        }

        public int CalculateBonusMultiplier(IAbilityUser user) =>
            tenetConstraint.Satisfies(user, tenetType) ? user.TenetStatusEffectsContainer.GetTenetStatusCount(tenetType) : 0;
    }
}
