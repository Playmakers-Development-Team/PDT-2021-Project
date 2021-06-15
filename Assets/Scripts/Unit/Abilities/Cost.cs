using System;
using Unit.StatusEffects;
using UnityEngine;

namespace Unit.Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private CostType costType;
        [SerializeField] private TenetType tenetType;

        public CostType CostType => costType;
        public TenetType TenetType => tenetType;

        public int CalculateBonusMultiplier(IUnit user)
        {
            if (costType == CostType.Per)
                return user.GetTenetStatusEffectCount(tenetType);
            
            return 0;
        }

        public void Expend(IUnit user)
        {
            switch (costType)
            {
                case CostType.Per:
                    user.RemoveTenetStatusEffect(tenetType);
                    break;
                case CostType.Spend:
                    user.RemoveTenetStatusEffect(tenetType, 1);
                    break;
            }
        }
        
        public bool MeetsRequirements(IUnit user) => user.GetTenetStatusEffectCount(tenetType) > 0;
    }
}
