using System;
using UnityEngine;

namespace GridObjects
{
    [Serializable]
    public class ModifierStat
    {
        [field: SerializeField] public float BaseMultiplier { get; set; }
        [field: SerializeField] public float BaseAdder { get; set; }
        
        public float Multiplier { get; set; }
        public float Adder { get; set; }

        public float Modify(float amount) => amount * Multiplier + Adder;

        public void Reset()
        {
            Multiplier = BaseMultiplier;
            Adder = BaseAdder;
        }
    }
}
