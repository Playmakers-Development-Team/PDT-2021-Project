using System;
using System.Linq;
using TenetStatuses;
using UnityEngine;
using Utilities;

namespace Abilities.Costs
{
    [Serializable]
    public enum TenetCostType
    {
        With, Consume, Spend
    }
    
    [Serializable]
    public class TenetCost : ICost, ISerializationCallbackReceiver
    {
        [SerializeField] private TenetCostType tenetCostType;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField] private TenetType tenetType;
        [SerializeField] private TenetConstraint tenetConstraint;

        public TenetCostType TenetCostType => tenetCostType;
        public TenetType TenetType => tenetType;

        public string DisplayName
        {
            get
            {
                string constraintString = tenetConstraint == TenetConstraint.None
                    ? string.Empty
                    : $" {StringUtility.UppercaseToReadable(tenetConstraint)}";
                return $"{tenetCostType} {count} {tenetType}{constraintString}";
            }
        }

        public void ApplyCost(IAbilityUser unit)
        {
            switch (TenetCostType)
            {
                case TenetCostType.Consume:
                    unit.RemoveTenetStatus(TenetType);
                    break;
                case TenetCostType.Spend:
                    unit.RemoveTenetStatus(TenetType, count);
                    break;
            }
        }
        
        public bool MeetsRequirements(IAbilityUser user) => 
            tenetConstraint.Satisfies(user, tenetType) && user.GetTenetStatusCount(tenetType) >= count;

        public void OnBeforeSerialize()
        {
            // We need this to circumvent unity issue that does not set default values
            count = Mathf.Max(1, count);
        }

        public void OnAfterDeserialize() {}
    }
}
