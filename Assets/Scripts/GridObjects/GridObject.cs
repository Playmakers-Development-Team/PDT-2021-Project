using System;
using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        public Stat TakeDamageModifier { get; protected set; }
        public Stat TakeKnockbackModifier { get; protected set; }

        public Vector2Int Coordinate =>
            gridManager.ConvertWorldSpaceToGridSpace(transform.position);

        private GridManager gridManager;

        public void TakeDamage(int amount)
        {
            int damageTaken = (int) TakeDamageModifier.Modify(amount);
            Debug.Log(damageTaken + " damage taken.");
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }
        
        public Vector2Int GetGridPosition(Vector2 worldPosition)
        {
            return gridManager.ConvertWorldSpaceToGridSpace(worldPosition);
        }
        
        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            gridManager.AddGridObject(Coordinate, this);
        }
    }
}
