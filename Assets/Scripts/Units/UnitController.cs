using System;
using UnityEngine;

namespace Units
{
    public abstract class UnitController<T> : MonoBehaviour, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
    }
}
