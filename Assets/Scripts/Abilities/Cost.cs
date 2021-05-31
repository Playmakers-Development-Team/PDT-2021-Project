using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private CostType costType;
        [SerializeField] private TenetType tenetType;

        public CostType CostType => costType;
        public TenetType TenetType => tenetType;

        public int CalculateBonusValue(IUnit user)
        {
            if (costType == CostType.Per)
                return user.GetTenetStatusEffectCount(tenetType);
            
            return 0;
        }

        public void Expend(IUnit user)
        {
            if (costType == CostType.Per)
                user.RemoveTenetStatusEffect(tenetType, 1);
        }
        
        public bool MeetsRequirements(IUnit user)
        {
            if (costType == CostType.Per)
                return true;

            return user.GetTenetStatusEffectCount(tenetType) > 0;
        }
    }
}
