using System;
using UnityEngine;

namespace TenetStatuses
{
    [Serializable]
    public struct TenetStatus
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

        public TenetStatus(TenetType tenetType, int stackCount)
        {
            this.tenetType = tenetType;
            this.stackCount = 0;
            StackCount = stackCount;
        }
        
        public static TenetStatus operator-(TenetStatus first, TenetStatus second)
        {
            if (first.TenetType != second.TenetType)
            {
                throw new ArithmeticException(
                    $"Cannot subtract tenets of differing types {first.TenetType} and {second.TenetType}");
            }

            return new TenetStatus(first.TenetType, first.StackCount - second.StackCount);
        }

        public static TenetStatus operator+(TenetStatus first, TenetStatus second)
        {
            if (first.TenetType != second.TenetType)
            {
                throw new ArithmeticException(
                    $"Cannot add tenets of differing types {first.TenetType} and {second.TenetType}");
            }

            return new TenetStatus(first.TenetType, first.StackCount + second.StackCount);
        }
        
        public static TenetStatus operator +(TenetStatus first, int amount) => 
            new TenetStatus(first.TenetType, first.StackCount + amount);
        
        public static TenetStatus operator -(TenetStatus first, int amount) => 
            new TenetStatus(first.TenetType, first.StackCount - amount);
        
        public static TenetStatus operator *(TenetStatus first, int amount) => 
            new TenetStatus(first.TenetType, first.StackCount * amount);
        
        public static TenetStatus operator /(TenetStatus first, int amount) => 
            new TenetStatus(first.TenetType, first.StackCount / amount);

        public static bool operator ==(TenetStatus first, TenetStatus second) =>
            first.TenetType == second.TenetType && first.StackCount == second.StackCount;

        public static bool operator !=(TenetStatus first, TenetStatus second) => 
            first.TenetType != second.TenetType || first.StackCount != second.StackCount;
    }
}
