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
    public class CompositeBonus : Conditional, IDisplayable
    {
        [SerializeField] protected BonusType bonusType;
        
        [CompositeChild((int) BonusType.Tenet)]
        [SerializeField] protected TenetBonus tenetBonus;
        // Put more types of bonuses here, they need to be protected to be read by the property drawer
        
        public string DisplayName => ChildBonus.DisplayName;

        public BonusType BonusType => bonusType;

        protected virtual IBonus ChildBonus => bonusType switch
        {
            BonusType.Tenet => tenetBonus,
            _ => throw new ArgumentOutOfRangeException(nameof(bonusType), bonusType, null)
        };

        /// <summary>
        /// Calculates the multiplier based on all bonuses.
        /// If there aren't any, returns a multiplier of zero.
        /// </summary>
        public int CalculateBonusMultiplier(IAbilityUser user, IAbilityUser target) =>
            bonusType != BonusType.None
                ? ChildBonus.CalculateBonusMultiplier(GetAffectedUser(user, target))
                : 0;
    }
}
