using System;
using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        private int HealthPoints { get; set; }
        private int MovementActionPoints { get; set; }
        private int Speed { get; set; }
        public Stat TakeDamageModifier { get; protected set; }
        public Stat TakeKnockbackModifier { get; protected set; }
        
        // TODO Initialise position
        private Vector2Int position;
        
        private GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            position = gridManager.ConvertPositionToCoordinate(transform.position);

            gridManager.AddGridObject(position, this);
        }

        public void TakeDamage(int amount)
        {
            int damageTaken = (int) TakeDamageModifier.Modify(amount);
            HealthPoints = damageTaken;
            CheckDeath();
            Debug.Log(damageTaken + " damage taken.");
            Debug.Log($"Health Before: {HealthPoints + damageTaken}  |  Health After: {HealthPoints}");
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }

        public Vector2Int GetGridPosition(Vector2 worldPosition)
        {
            return gridManager.ConvertPositionToCoordinate(worldPosition);
        }

        public void CheckDeath()
        {
            if (HealthPoints <= 0)
                Debug.Log($"This Grid Object was cringe and died");
        }
    }
}
