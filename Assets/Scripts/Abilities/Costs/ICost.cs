namespace Abilities.Costs
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeCost"/>
    /// </summary>
    public interface ICost : IDisplayable
    {
        void ApplyCost(IAbilityUser unit);

        bool MeetsRequirements(IAbilityUser unit);
    }
}
