using System;
using UnityEngine;

namespace Units
{
    public abstract class UnitBase<T> : MonoBehaviour, IUnit where T : UnitDataBase
    {
        [SerializeField] protected T data;
        
        public Vector3 Position => transform.position;
        
        public static Type DataType => typeof(T);
    }
}
