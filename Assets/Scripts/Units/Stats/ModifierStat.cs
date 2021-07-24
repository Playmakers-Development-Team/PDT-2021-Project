using System;
using UnityEngine;

namespace Units.Stats
{
    [Serializable][Obsolete("Modifier stat is not being used, use the Stat Class instead")]
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

        public float Value => Adder * Multiplier;
    }
}
