using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject
    {
        private Vector2Int position;

        private int HealthPoints { get; set; }
        private int MovementActionPoints { get; set; }
        public Stat DealDamageModifier { get; }
        public Stat TakeDamageModifier { get; }
        public Stat TakeKnockbackModifier { get; }
        
        private GridManager gridManager;

        public GridObject(
            int healthPoints,
            int movementActionPoints,
            Vector2Int position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        )
        {
            this.position = position;
            HealthPoints = healthPoints;
            MovementActionPoints = movementActionPoints;
            DealDamageModifier = dealDamageModifier;
            TakeDamageModifier = takeDamageModifier;
            TakeKnockbackModifier = takeKnockbackModifier;

            gridManager = ManagerLocator.Get<GridManager>();

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
            return gridManager.ConvertWorldSpaceToGridSpace(worldPosition);
        }

        public void CheckDeath()
        {
            if (HealthPoints <= 0)
                Debug.Log($"This Grid Object was cringe and died");
        }
        
    }
}
