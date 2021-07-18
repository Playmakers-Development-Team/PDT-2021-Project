using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable][Obsolete("ValueStat is not being used, use the Stat Class instead")]
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
