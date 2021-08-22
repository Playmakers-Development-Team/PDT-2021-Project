using Abilities.Parsing;

namespace Abilities.Bonuses
{
    /// <summary>
    /// A handy interface for better structuring. Please see <see cref="CompositeBonus"/>
    /// </summary>
    public interface IBonus : IDisplayable
    {
        int CalculateBonusMultiplier(IAbilityContext context, IAbilityUser user);
    }
}
