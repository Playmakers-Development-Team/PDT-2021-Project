using System;
using UnityEngine;

namespace GridObjects
{
    [Serializable]
    public class ValueStat
    {
        [field: SerializeField] public float BaseValue { get; set; }
        
        public float Value { get; set; }
        
        public void Reset()
        {
            Value = BaseValue;
        }
    }
}