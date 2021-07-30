using System;
using System.Collections.Generic;
using System.Linq;
using TenetStatuses;

namespace Abilities.Costs
{
    public enum TenetTarget
    {
        All, FirstToLast, LastToFirst, Newest, Oldest
    }

    public static class TenetTargetExtensions
    {
        public static TenetType? GetSpecificTenet(this TenetTarget tenetTarget, IAbilityUser user)
        {
            return tenetTarget switch
            {
                TenetTarget.Newest => user.TenetStatusEffectsContainer.TenetStatuses.Count > 0
                    ? user.TenetStatusEffectsContainer.TenetStatuses.Last().TenetType
                    : (TenetType?) null,
                TenetTarget.Oldest => user.TenetStatusEffectsContainer.TenetStatuses.Count > 0
                    ? user.TenetStatusEffectsContainer.TenetStatuses.First().TenetType
                    : (TenetType?) null,
                TenetTarget.LastToFirst => user.TenetStatusEffectsContainer.TenetStatuses.Count > 0
                    ? user.TenetStatusEffectsContainer.TenetStatuses.Last().TenetType
                    : (TenetType?) null,
                TenetTarget.FirstToLast => user.TenetStatusEffectsContainer.TenetStatuses.Count > 0
                    ? user.TenetStatusEffectsContainer.TenetStatuses.First().TenetType
                    : (TenetType?) null,
                _ => null
            };
        }
    }
}
