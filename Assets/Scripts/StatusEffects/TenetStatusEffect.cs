using System;

namespace StatusEffects
{
    public struct TenetStatusEffect
    {
        public TenetType TenetType { get; }

        public int StackCount
        {
            get => stackCount;
            private set => stackCount = Math.Max(0, value);
        }

        public bool IsEmpty => StackCount <= 0;
        
        private int stackCount;

        public TenetStatusEffect(TenetType tenetType, int stackCount)
        {
            TenetType = tenetType;
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
