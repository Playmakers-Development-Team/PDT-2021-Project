using System;
using TenetStatuses;
using UnityEngine;

namespace Abilities.Bonuses
{
    [Serializable]
    public class TenetBonus : IBonus
    {
        [SerializeField] private TenetType tenetType;

        public string DisplayName => tenetType.ToString();
        
        public int CalculateBonusMultiplier(IAbilityUser user) =>
            user.TenetStatusEffectsContainer.GetTenetStatusCount(tenetType);
    }
}
