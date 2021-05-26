using UnityEngine;

namespace GridObjects
{
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
