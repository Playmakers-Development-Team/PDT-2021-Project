using System.Collections.Generic;
using System.Linq;

namespace TenetStatuses
{
    public class TenetStatusEffectsContainer : ITenetBearer
    {
        private const int maxTenetStatusEffectCount = 2;
        private LinkedList<TenetStatus> tenetStatusEffectSlots =
            new LinkedList<TenetStatus>();
        
        public ICollection<TenetStatus> TenetStatuses => tenetStatusEffectSlots;

        public TenetStatusEffectsContainer() {}

        public TenetStatusEffectsContainer(IEnumerable<TenetStatus> tenets) => Initialise(tenets);

        public void Initialise(IEnumerable<TenetStatus> startingTenets) =>
            tenetStatusEffectSlots = new LinkedList<TenetStatus>(startingTenets);

        public void SetTenets(ITenetBearer tenetBearer)
        {
            Initialise(tenetStatusEffectSlots);
            // Might want to call a command that tenets have changed here
        }

        public void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1)
        {
            TenetStatus status = new TenetStatus(tenetType, stackCount);

            if (status.IsEmpty)
                return;

            // Try to add on top of an existing tenet type
            if (TryGetTenetStatusNode(status.TenetType, out LinkedListNode<TenetStatus> foundNode))
                foundNode.Value += status;
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

        public bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus)
        {
            bool isFound = TryGetTenetStatusNode(tenetType,
                out LinkedListNode<TenetStatus> foundNode);
            tenetStatus = isFound ? foundNode.Value : default;
            return isFound;
        }

        public int GetTenetStatusCount(TenetType tenetType) =>
            HasTenetStatus(tenetType)
                ? tenetStatusEffectSlots.Where(s => s.TenetType == tenetType).Sum(s => s.StackCount)
                : 0;

        public bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1) =>
            tenetStatusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);

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
