using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable]
    public class ValueStat
    {
        [field: SerializeField] public int BaseValue { get; set; }
        
        [field: SerializeField]public int Value { get; set; }
        
        public void Reset()
        {
            Value = BaseValue;
        }
    }
}
