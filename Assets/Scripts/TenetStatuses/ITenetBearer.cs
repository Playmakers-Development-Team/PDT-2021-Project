using System.Collections.Generic;

namespace TenetStatuses
{
    public interface ITenetBearer
    {
        ICollection<TenetStatus> TenetStatuses { get; }
    
        void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1);
    
        bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatus();

        public int GetTenetStatusCount(TenetType tenetType);

        bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1);

        bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus);
    }
}
