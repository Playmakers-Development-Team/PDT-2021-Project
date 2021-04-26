using System;
using UnityEngine;

namespace Units
{
    public abstract class UnitBase<T> : MonoBehaviour, IUnit where T : UnitDataBase
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
    }
}
