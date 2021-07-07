using Units;

namespace Abilities.Conditionals
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeBonus"/>
    /// </summary>
    public interface IBonus
    {
        int CalculateBonusMultiplier(IUnit user, IUnit target);
    }
}
