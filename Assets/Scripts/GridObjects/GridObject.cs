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

        public ModifierStat TakeDamageModifier { get; protected set; }
        public ModifierStat TakeKnockbackModifier { get; protected set; }
        
        // TODO Initialise position
        private Vector2Int position;

        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);
        
        private GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            gridManager.AddGridObject(Coordinate, this);
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
