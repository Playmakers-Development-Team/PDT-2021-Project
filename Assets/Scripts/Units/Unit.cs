using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using GridObjects;
using StatusEffects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);

        public ModifierStat DealDamageModifier { get; protected set; }
        public ValueStat Speed { get; protected set; }
        
        public Type GetDataType() => DataType;
        
        public int TenetStatusEffectCount => tenetStatusEffectSlots.Count;

        public IEnumerable<TenetStatusEffect> TenetStatusEffects => tenetStatusEffectSlots.AsEnumerable();
        
        private readonly LinkedList<TenetStatusEffect> tenetStatusEffectSlots = new LinkedList<TenetStatusEffect>();
        private const int maxTenetStatusEffectCount = 2;
        
        protected override void Start()
        {
            base.Start();
            
            data.Initialise();
            
            Speed = data.speed;
            DealDamageModifier = data.dealDamageModifier;
            TakeDamageModifier = data.takeDamageModifier;
            TakeKnockbackModifier = data.takeKnockbackModifier;
            
            // TODO Are speeds are random or defined in UnitData?
            Speed.Value += Random.Range(10,50);

            DealDamageModifier.Reset();
            TakeDamageModifier.Reset();
            TakeKnockbackModifier.Reset();
        }

        public void AddDefence(int amount) => data.AddDefence(amount);
        public void AddAttack(int amount) => data.AddAttack(amount);

        public void Knockback(Vector2Int translation) => throw new NotImplementedException();

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

        public int GetTenetStatusEffectCount(TenetType tenetType)
        {
            return HasTenetStatusEffect(tenetType)
                ? tenetStatusEffectSlots.Where(s => s.TenetType == tenetType).Sum(s => s.StackCount)
                : 0;
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
