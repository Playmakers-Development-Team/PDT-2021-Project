using GridObjects;
using UnityEngine;

namespace Units
{
    public interface IUnit
    {
        public Stat DealDamageModifier { get; }
        
        public Vector2Int Coordinate { get; }
    }
}
