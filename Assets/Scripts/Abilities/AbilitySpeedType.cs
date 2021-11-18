using System;

namespace Abilities
{
    public enum AbilitySpeedType
    {
        VeryHeavy = 3,
        Heavy = 1,
        Average = 0,
        Lightweight = 2,
        VeryLightweight = 4
    }

    public static class AbilitySpeedTypeExtensions
    {
        public static string DisplayName(this AbilitySpeedType abilitySpeedType) => abilitySpeedType switch
        {
            AbilitySpeedType.VeryHeavy => "Very Heavy",
            AbilitySpeedType.Heavy => "Heavy",
            AbilitySpeedType.Average => "Average",
            AbilitySpeedType.Lightweight => "Lightweight",
            AbilitySpeedType.VeryLightweight => "Very Lightweight",
            _ => throw new ArgumentOutOfRangeException(nameof(abilitySpeedType), abilitySpeedType, null)
        };
    }
}
