using System;
using System.Collections.Generic;
using System.Linq;

namespace TenetStatuses
{
    public class TenetStatusEffectsContainer
    {
        private const int maxTenetStatusEffectCount = 2;
        private readonly LinkedList<TenetStatus> tenetStatusEffectSlots =
            new LinkedList<TenetStatus>();
        
        [Obsolete("Use TenetStatuses instead")]
        public ICollection<TenetStatus> TenetStatusEffects => TenetStatuses;
        public ICollection<TenetStatus> TenetStatuses => tenetStatusEffectSlots;
        
        public void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1)
        {
            TenetStatus status = new TenetStatus(tenetType, stackCount);

            if (status.IsEmpty)
                return;

            // Try to add on top of an existing tenet type
            if (TryGetTenetStatusNode(status.TenetType, out LinkedListNode<TenetStatus> foundNode))
            {
                foundNode.Value += status;
            }
            else
            {
                // When we are already utilizing all the slots
                if (TenetStatuses.Count == maxTenetStatusEffectCount)
                {
                    // Remove the oldest status effect to make space for the new status effect
                    tenetStatusEffectSlots.RemoveFirst();
                }

                tenetStatusEffectSlots.AddLast(status);
            }
        }

        public bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue)
        {
            LinkedListNode<TenetStatus> node = tenetStatusEffectSlots.First;

            while (node != null)
            {
                if (node.Value.TenetType == tenetType)
                {
                    node.Value -= amount;

                    if (node.Value.IsEmpty)
                        tenetStatusEffectSlots.Remove(node);
                    
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public void ClearAllTenetStatus() => tenetStatusEffectSlots.Clear();

        [Obsolete("Use TryGetTenetStatus instead")]
        public bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus) =>
            TryGetTenetStatusEffect(tenetType, out tenetStatus);

        public bool TryGetTenetStatusEffect(TenetType tenetType,
                                            out TenetStatus tenetStatus)
        {
            bool isFound = TryGetTenetStatusNode(tenetType,
                out LinkedListNode<TenetStatus> foundNode);
            tenetStatus = isFound ? foundNode.Value : default;
            return isFound;
        }

        [Obsolete("Use GetTenetStatus instead")]
        public int GetTenetStatusEffectCount(TenetType tenetType) =>
            GetTenetStatusCount(tenetType);

        public int GetTenetStatusCount(TenetType tenetType)
        {
            return HasTenetStatus(tenetType)
                ? tenetStatusEffectSlots.Where(s => s.TenetType == tenetType).Sum(s => s.StackCount)
                : 0;
        }

        [Obsolete("Use HasTenetStatus instead")]
        public bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1) =>
            HasTenetStatus(tenetType, minimumStackCount);

        public bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1)
        {
            return tenetStatusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);
        }

        private bool TryGetTenetStatusNode(TenetType tenetType,
                                           out LinkedListNode<TenetStatus> foundNode)
        {
            LinkedListNode<TenetStatus> node = tenetStatusEffectSlots.First;

            while (node != null)
            {
                if (node.Value.TenetType == tenetType)
                {
                    foundNode = node;
                    return true;
                }

                node = node.Next;
            }

            foundNode = null;
            return false;
        }
    }
}
