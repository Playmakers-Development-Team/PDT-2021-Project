using System;
using Units;
using Units.TenetStatuses;
using UnityEngine;

namespace Abilities.Bonuses
{
    [Serializable]
    public class TenetBonus : Conditional
    {
        [SerializeField] private TenetType tenetType;
        
        public int CalculateBonusMultiplier(IUnit user, IUnit target) =>
            GetAffectedUnit(user, target).GetTenetStatusCount(tenetType);
    }
}
