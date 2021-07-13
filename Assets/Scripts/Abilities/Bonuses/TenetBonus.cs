using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Bonuses
{
    [Serializable]
    public class TenetBonus : IBonus
    {
        [SerializeField] private TenetType tenetType;

        public string DisplayName => tenetType.ToString();
        
        public int CalculateBonusMultiplier(IUnit unit) =>
            unit.GetTenetStatusCount(tenetType);
    }
}
