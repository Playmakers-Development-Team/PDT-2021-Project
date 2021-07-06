using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Costs
{
    [Serializable]
    public enum TenetCostType
    {
        With, Per, Spend
    }
    
    [Serializable]
    public class TenetCost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private TenetCostType tenetCostType;
        [SerializeField] private TenetType tenetType;
        
        public TenetCostType TenetCostType => tenetCostType;
        public TenetType TenetType => tenetType;

        public void ExpendTenet(IUnit unit)
        {
            switch (TenetCostType)
            {
                case TenetCostType.Per:
                    unit.RemoveTenetStatus(TenetType);
                    break;
                case TenetCostType.Spend:
                    unit.RemoveTenetStatus(TenetType, 1);
                    break;
            }
        }
    }
}
