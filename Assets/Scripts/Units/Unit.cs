using System;
using GridObjects;
using UnityEngine;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
        
        public Stat DealDamageModifier { get; protected set; }

        protected override void Start()
        {
            base.Start();
            
            DealDamageModifier = data.dealDamageModifier;
            TakeDamageModifier = data.takeDamageModifier;
            TakeKnockbackModifier = data.takeKnockbackModifier;
        }
    }
}
