using TenetStatuses;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// <p>Takes the stats and tenets of a unit and provide an easy way to modify them.
    /// When finally satisfied with modification, it can set them in one single step</p>
    /// </summary>
    public interface IVirtualAbilityUser : IAbilityUser
    {
        IAbilityUser RealAbilityUser { get; }
        
        void ApplyChanges();
    }
}
