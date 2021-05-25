using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
        
        public Stat DealDamageModifier { get; protected set; }
        
        public int TenetStatusEffectCount => tenetStatusEffectSlots.Count;

        public IEnumerable<TenetStatusEffect> TenetStatusEffects => tenetStatusEffectSlots.AsEnumerable();
        
        private readonly LinkedList<TenetStatusEffect> tenetStatusEffectSlots = new LinkedList<TenetStatusEffect>();
        private const int maxTenetStatusEffectCount = 2;
        
        protected override void Start()
        {
            DealDamageModifier = data.dealDamageModifier;
            TakeDamageModifier = data.takeDamageModifier;
            TakeKnockbackModifier = data.takeKnockbackModifier;
        }

        public void AddOrReplaceTenetStatusEffect(TenetType tenetType, int stackCount = 1)
        {
            TenetStatusEffect statusEffect = new TenetStatusEffect(tenetType, stackCount);

            if (statusEffect.IsEmpty)
                return;
            
            // Try to add on top of an existing tenet type
            if (TryGetTenetStatusEffectNode(statusEffect.TenetType, out LinkedListNode<TenetStatusEffect> foundNode))
            {
                foundNode.Value += statusEffect;
            }
            else
            {
                // When we are already utilizing all the slots
                if (TenetStatusEffectCount == maxTenetStatusEffectCount)
                {
                    // Remove the oldest status effect to make space for the new status effect
                    tenetStatusEffectSlots.RemoveFirst();
                }
                
                tenetStatusEffectSlots.AddLast(statusEffect);
            }
        }

        public bool RemoveTenetStatusEffect(TenetType tenetType, int amount = int.MaxValue)
        {
            LinkedListNode<TenetStatusEffect> node = tenetStatusEffectSlots.First;

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

        public void ClearAllTenetStatusEffects()
        {
            tenetStatusEffectSlots.Clear();
        }

        public bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatusEffect tenetStatusEffect)
        {
            bool isFound = TryGetTenetStatusEffectNode(tenetType, out LinkedListNode<TenetStatusEffect> foundNode);
            tenetStatusEffect = isFound ? foundNode.Value : default;
            return isFound;
        }

        public bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1)
        {
            return tenetStatusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);
        }

        private bool TryGetTenetStatusEffectNode(TenetType tenetType,
                                                 out LinkedListNode<TenetStatusEffect> foundNode)
        {
            LinkedListNode<TenetStatusEffect> node = tenetStatusEffectSlots.First;

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
