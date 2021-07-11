using System;
using UnityEngine;

namespace GridObjects
{
    [Serializable]
    public class ValueStat
    {
        [field: SerializeField] public int BaseValue { get; set; }
        
        public int Value { get; set; }
        
        public void Reset()
        {
            Value = BaseValue;
        }
    }
}
