using System;
using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        public Stat TakeDamageModifier { get; protected set; }
        public Stat TakeKnockbackModifier { get; protected set; }
        
        private GridManager gridManager;
        
        // TODO Initialise position
        private Vector2Int position;

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

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            position = gridManager.ConvertWorldSpaceToGridSpace(transform.position);

            gridManager.AddGridObject(position, this);
        }
    }
}
