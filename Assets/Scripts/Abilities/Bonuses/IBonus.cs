using Units;

namespace Abilities.Bonuses
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeBonus"/>
    /// </summary>
    public interface IBonus
    {
        int CalculateBonusMultiplier(IUnit user, IUnit target);
    }
}
