using System;
using System.Linq;
using TenetStatuses;

namespace Abilities
{
    public enum TenetConstraint
    {
        None, WhenOldest, WhenNewest
    }

    public static class TenetContraintExtensions
    {
        public static bool Satisfies(this TenetConstraint tenetConstraint, 
                                     IAbilityUser user, TenetType tenetType) => 
            tenetConstraint switch
            {
                TenetConstraint.None => true,
                TenetConstraint.WhenNewest => 
                    user.TenetStatusEffectsContainer.TenetStatuses.Count > 0 && user.TenetStatusEffectsContainer.TenetStatuses.Last().TenetType == tenetType,
                TenetConstraint.WhenOldest => 
                    user.TenetStatusEffectsContainer.TenetStatuses.Count > 0 && user.TenetStatusEffectsContainer.TenetStatuses.First().TenetType == tenetType,
                _ => throw new ArgumentOutOfRangeException(nameof(tenetConstraint), tenetConstraint, null)
            };
    }
}
