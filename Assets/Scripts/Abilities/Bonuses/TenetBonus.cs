using System;
using TenetStatuses;
using UnityEngine;

namespace Abilities.Bonuses
{
    [Serializable]
    public class TenetBonus : Conditional
    {
        [SerializeField] private TenetType tenetType;
        
        public int CalculateBonusMultiplier(IAbilityUser user, IAbilityUser target) =>
            GetAffectedUser(user, target).GetTenetStatusCount(tenetType);
    }
}
