using System;
using UnityEngine;

namespace StatusEffects
{
    [Serializable]
    public struct TenetStatusEffect
    {
        [SerializeField] private TenetType tenetType;
        [SerializeField] private int stackCount;

        public TenetType TenetType => tenetType;

        public int StackCount
        {
            get => stackCount;
            private set => stackCount = Math.Max(0, value);
        }

        public bool IsEmpty => StackCount <= 0;

        public TenetStatusEffect(TenetType tenetType, int stackCount)
        {
            this.tenetType = tenetType;
            this.stackCount = 0;
            StackCount = stackCount;
        }
        
        public static TenetStatusEffect operator-(TenetStatusEffect first, TenetStatusEffect second)
        {
            if (first.TenetType != second.TenetType)
            {
                throw new ArithmeticException(
                    $"Cannot subtract tenets of differing types {first.TenetType} and {second.TenetType}");
            }

            return new TenetStatusEffect(first.TenetType, first.StackCount - second.StackCount);
        }

        public static TenetStatusEffect operator+(TenetStatusEffect first, TenetStatusEffect second)
        {
            if (first.TenetType != second.TenetType)
            {
                throw new ArithmeticException(
                    $"Cannot add tenets of differing types {first.TenetType} and {second.TenetType}");
            }

            return new TenetStatusEffect(first.TenetType, first.StackCount + second.StackCount);
        }
        
        public static TenetStatusEffect operator +(TenetStatusEffect first, int amount) => 
            new TenetStatusEffect(first.TenetType, first.StackCount + amount);
        
        public static TenetStatusEffect operator -(TenetStatusEffect first, int amount) => 
            new TenetStatusEffect(first.TenetType, first.StackCount - amount);
        
        public static TenetStatusEffect operator *(TenetStatusEffect first, int amount) => 
            new TenetStatusEffect(first.TenetType, first.StackCount * amount);
        
        public static TenetStatusEffect operator /(TenetStatusEffect first, int amount) => 
            new TenetStatusEffect(first.TenetType, first.StackCount / amount);
    }
}
