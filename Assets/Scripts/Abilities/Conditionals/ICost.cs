using Units;

namespace Abilities.Conditionals
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
