using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abilities.Bonuses
{
    /// <summary>
    /// <p> Bonuses act like an additional value on top of existing values based on some condition </p>
    /// 
    /// <p> This is a convenient class that stores all the different type of bonuses into one class.
    /// The Composite Pattern for a better structure and organization.</p>
    /// </summary>
    [Serializable]
    public class CompositeBonus : IBonus
    {
        [SerializeField] private List<TenetBonus> tenetBonuses;
        // Add more types of bonuses here

        /// <summary>
        /// Calculates the multiplier based on all bonuses.
        /// If there aren't any just return a multiplier of 1.
        /// </summary>
        public int CalculateBonusMultiplier(IAbilityUser user, IAbilityUser target) =>
            tenetBonuses.Count > 0
                ? tenetBonuses.Sum(b => b.CalculateBonusMultiplier(user, target))
                : 1;
    }
}
