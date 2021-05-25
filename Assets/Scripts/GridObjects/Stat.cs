using System;
using UnityEngine;

namespace GridObjects
{
    [Serializable]
    public class Stat
    {
        [SerializeField] private float baseMultiplier;
        [SerializeField] private float baseAdder;
        
        private float multiplier;
        private float adder;

        // TODO: Create a setup function
        public Stat(float baseMultiplier, float baseAdder)
        {
            this.baseMultiplier = baseMultiplier;
            this.baseAdder = baseAdder;
            
            multiplier = baseMultiplier;
            adder = baseAdder;
        }

        public float Modify(float amount) => amount * multiplier + adder;

        public float Value => baseAdder * baseMultiplier;
    }
}
