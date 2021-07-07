using System;
using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEngine;

namespace Abilities.Conditionals
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

        public int CalculateBonusMultiplier(IUnit user, IUnit target) =>
            tenetBonuses.Sum(b => b.CalculateBonusMultiplier(user, target));
    }
}
