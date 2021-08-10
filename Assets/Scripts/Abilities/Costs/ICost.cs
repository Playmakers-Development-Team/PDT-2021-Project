using Abilities.Parsing;

namespace Abilities.Costs
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeCost"/>
    /// </summary>
    public interface ICost : IDisplayable
    {
        void ApplyCost(IAbilityContext context, IAbilityUser user);

        bool MeetsRequirements(IAbilityContext context, IAbilityUser abilityUser);
    }
}
