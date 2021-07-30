using System;
using System.Collections.Generic;

namespace TenetStatuses
{
    public interface ITenetBearer
    {
        [Obsolete("Use TenetStatuses instead")]
        ICollection<TenetStatus> TenetStatusEffects { get; }
        ICollection<TenetStatus> TenetStatuses { get; }
    
        void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1);
    
        bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatus();

        [Obsolete("Use GetTenetStatusCount instead")]
        public int GetTenetStatusEffectCount(TenetType tenetType);
    
        public int GetTenetStatusCount(TenetType tenetType);

        [Obsolete("Use HasTenetStatus instead")]
        bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1);
    
        bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1);

        [Obsolete("Use TryGetTenetStatus instead")]
        bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus);
    
        bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatus tenetStatus);
    }
}
