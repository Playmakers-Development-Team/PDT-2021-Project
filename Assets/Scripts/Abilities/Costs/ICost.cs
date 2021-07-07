using Units;

namespace Abilities.Costs
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeCost"/>
    /// </summary>
    public interface ICost
    {
        void ApplyCost(IUnit user, IUnit target);

        bool MeetsRequirements(IUnit user, IUnit target);
    }
}
