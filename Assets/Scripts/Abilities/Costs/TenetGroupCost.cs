using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Parsing;
using TenetStatuses;
using UnityEngine;
using Utilities;

namespace Abilities.Costs
{
    [Serializable]
    public class TenetGroupCost : ICost, ISerializationCallbackReceiver
    {
        [SerializeField] private TenetTarget tenetTarget;
        [SerializeField] private TenetCostType tenetCostType;
        [SerializeField, Min(1)] private int count = 1;
        [SerializeField] private TenetMask tenetFilter;

        public string DisplayName => $"{tenetCostType} {StringUtility.UppercaseToReadable(tenetTarget)} from any {count} of {tenetFilter.ToDisplayName()}";

        public void ApplyCost(IAbilityContext context, IAbilityUser user)
        {
            List<TenetType> allTypes = new List<TenetType>();
            
            if (tenetTarget == TenetTarget.All)
            {
                // var tenetTypes = GetMatchingTenets(user.TenetStatuses)
                //     .Select(t => t.TenetType);
                // allTypes.AddRange(tenetTypes);
                
                // Spend or Consume on this is not implemented and is not needed.
                // It might take a while to come up with a reasonable outcome/implementation for this.
                Debug.LogWarning("Cannot spend or consume targeting all tenets!");
            }
            else
            {
                TenetType? tenetType = tenetTarget.GetSpecificTenet(user);

                if (tenetType.HasValue)
                    allTypes.Add(tenetType.Value);
            }

            foreach (TenetType tenetType in allTypes)
            {
                switch (tenetCostType)
                {
                    case TenetCostType.Spend:
                        user.RemoveTenetStatus(tenetType, count);
                        break;
                    case TenetCostType.Consume:
                        user.RemoveTenetStatus(tenetType);
                        break;
                }
            }
        }

        public bool MeetsRequirements(IAbilityContext context, IAbilityUser user) => tenetTarget switch
        {
            TenetTarget.Newest => MatchSpecificTenet(user),
            TenetTarget.Oldest => MatchSpecificTenet(user),
            TenetTarget.All => MatchAllTenets(user),
            _ => throw new ArgumentOutOfRangeException(nameof(TenetTarget), tenetTarget, null)
        };

        private bool MatchSpecificTenet(IAbilityUser user)
        {
            TenetType? tenetType = tenetTarget.GetSpecificTenet(user);

            if (!tenetType.HasValue)
                return false;

            return tenetFilter.IsTenetInMask(tenetType.Value)
                   && user.GetTenetStatusCount(tenetType.Value) >= count;
        }

        private bool MatchAllTenets(IAbilityUser user) =>
            GetMatchingTenets(user.TenetStatuses).Sum(t => t.StackCount) >= count;
        
        private IEnumerable<TenetStatus> GetMatchingTenets(IEnumerable<TenetStatus> tenets) =>
            tenets.Where(t => tenetFilter.IsTenetInMask(t.TenetType));
        
        public void OnBeforeSerialize()
        {
            // We need this to circumvent unity issue that does not set default values
            count = Mathf.Max(1, count);
        }

        public void OnAfterDeserialize() {}
    }
}
