using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Costs
{
    [Serializable]
    public enum TenetCostType
    {
        With, Consume, Spend
    }
    
    [Serializable]
    public class TenetCost : ICost
    {
        [SerializeField] private TenetCostType tenetCostType;
        [SerializeField] private TenetType tenetType;
        
        public TenetCostType TenetCostType => tenetCostType;
        public TenetType TenetType => tenetType;
        
        public void ApplyCost(IUnit unit)
        {
            switch (TenetCostType)
            {
                case TenetCostType.Consume:
                    unit.RemoveTenetStatus(TenetType);
                    break;
                case TenetCostType.Spend:
                    unit.RemoveTenetStatus(TenetType, 1);
                    break;
            }
        }
        
        public bool MeetsRequirements(IUnit unit) => unit.GetTenetStatusCount(tenetType) > 0;
    }
}
