using System;
using Managers;
using UnityEngine;

namespace Units
{
    public abstract class UnitController<T> : MonoBehaviour, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
        
        private GridManager gridManager;
        private Unit unit;
        
        // BUG: Putting the following in Awake creates a race condition with GridController 
        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            unit = new Unit(
                data.healthPoints,
                data.movementActionPoints,
                data.speed,
                Vector2Int.one,
                data.dealDamageModifier,
                data.takeDamageModifier,
                data.takeKnockbackModifier
            );
        }
    }
}
