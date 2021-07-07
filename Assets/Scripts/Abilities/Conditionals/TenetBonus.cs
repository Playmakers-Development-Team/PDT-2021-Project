using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Conditionals
{
    [Serializable]
    public class TenetBonus : Conditional
    {
        [SerializeField] private TenetType tenetType;
        
        public int CalculateBonusMultiplier(IUnit user, IUnit target) =>
            GetAffectedUnit(user, target).GetTenetStatusCount(tenetType);
    }
}
