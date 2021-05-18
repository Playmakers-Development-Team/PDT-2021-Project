using System;
using Abilities;
using UnityEngine;

namespace Units
{
    public abstract class Unit<T> : MonoBehaviour, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public static Type DataType => typeof(T);
        

        public Type GetDataType() => DataType;
        
        public void Damage(int amount) => data.Damage(amount);
        public void Defend(int amount) => data.Defend(amount);
        
        public void Knockback(Vector2Int translation) => throw new NotImplementedException();
        
        public int GetStacks(Tenet tenet) => data.GetStacks(tenet);
        public void Expend(Tenet tenet, int amount) => data.Expend(tenet, amount);
    }
}
