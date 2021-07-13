using Units;

namespace Abilities.Costs
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeCost"/>
    /// </summary>
    public interface ICost : IDisplayable
    {
        void ApplyCost(IUnit unit);

        bool MeetsRequirements(IUnit unit);
    }
}
