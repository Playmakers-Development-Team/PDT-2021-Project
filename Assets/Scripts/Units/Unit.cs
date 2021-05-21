using System.Collections.Generic;
using System.Linq;
using GridObjects;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public class Unit : GridObject, IUnit
    {
        // [SerializeField] protected T data;
        // public static Type DataType => typeof(T);
        
        // In the future, we might want to support other types of status effect
        private readonly LinkedList<TenetStatusEffect> statusEffectSlots = new LinkedList<TenetStatusEffect>();
        private const int maxStatusEffectCount = 2;

        public int StatusEffectCount => statusEffectSlots.Count;

        public IEnumerable<TenetStatusEffect> StatusEffects => statusEffectSlots.AsEnumerable();

        public Unit(
        int healthPoints,
        int movementActionPoints,
        Vector2Int position,
        Stat dealDamageModifier,
        Stat takeDamageModifier,
        Stat takeKnockbackModifier) : base(position, dealDamageModifier, takeDamageModifier, takeKnockbackModifier) {}
        
        
      

        public void AddOrReplaceTenet(TenetType tenetType, int stackCount = 1)
        {
            AddOrReplaceStatusEffect(new TenetStatusEffect(tenetType, stackCount));
        }

        private void AddOrReplaceStatusEffect(TenetStatusEffect statusEffect)
        {
            if (statusEffect.IsEmpty)
                return;
            
            // Try to add on top of an existing tenet type
            if (TryGetTenetNode(statusEffect.TenetType, out LinkedListNode<TenetStatusEffect> foundNode))
            {
                foundNode.Value += statusEffect;
            }
            else
            {
                // When we are already utilizing all the slots
                if (StatusEffectCount == maxStatusEffectCount)
                {
                    // Remove the oldest status effect to make space for the new status effect
                    statusEffectSlots.RemoveFirst();
                }
                
                statusEffectSlots.AddLast(statusEffect);
            }
        }

        public bool RemoveTenet(TenetType tenetType, int amount = int.MaxValue)
        {
            LinkedListNode<TenetStatusEffect> node = statusEffectSlots.First;

            while (node != null)
            {
                TenetStatusEffect tenetStatusEffect = node.Value;

                if (tenetStatusEffect.TenetType == tenetType)
                {
                    node.Value -= amount;
                    
                    if (node.Value.IsEmpty)
                        statusEffectSlots.Remove(node);
                    return true;
                }
                
                node = node.Next;
            }

            return false;
        }

        public void ClearAllStatusEffects()
        {
            statusEffectSlots.Clear();
        }

        public bool TryGetTenet(TenetType tenetType, out TenetStatusEffect tenetStatusEffect)
        {
            bool isFound = TryGetTenetNode(tenetType, out LinkedListNode<TenetStatusEffect> foundNode);
            tenetStatusEffect = isFound ? foundNode.Value : default;
            return isFound;
        }

        private bool TryGetTenetNode(TenetType tenetType, out LinkedListNode<TenetStatusEffect> foundNode)
        {
            LinkedListNode<TenetStatusEffect> node = statusEffectSlots.First;

            while (node != null)
            {
                TenetStatusEffect currentStatusEffect = node.Value;

                if (currentStatusEffect.TenetType == tenetType)
                {
                    foundNode = node;
                    return true;
                }
                
                node = node.Next;
            }

            foundNode = null;
            return false;
        }

        public bool HasTenet(TenetType tenetType, int minimumStackCount = 1)
        {
            return statusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);
        }
    }
}
