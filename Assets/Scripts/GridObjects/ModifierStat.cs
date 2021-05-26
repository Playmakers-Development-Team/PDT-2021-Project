using System;
using UnityEngine;

namespace GridObjects
{
    [Serializable]
    public class ModifierStat
    {
        public float BaseMultiplier { get; set; }
        public float BaseAdder { get; set; }
        
        public float Multiplier { get; set; }
        public float Adder { get; set; }

        public ModifierStat(float baseMultiplier, float baseAdder)
        {
            BaseMultiplier = baseMultiplier;
            BaseAdder = baseAdder;

            Reset();
        }

        public float Modify(float amount) => amount * Multiplier + Adder;

        public void Reset()
        {
            Multiplier = BaseMultiplier;
            Adder = BaseAdder;
        }
    }
}
