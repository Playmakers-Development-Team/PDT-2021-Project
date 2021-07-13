namespace Abilities.Costs
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeCost"/>
    /// </summary>
    public interface ICost
    {
        void ApplyCost(IAbilityUser user, IAbilityUser target);

        bool MeetsRequirements(IAbilityUser user, IAbilityUser target);
    }
}
