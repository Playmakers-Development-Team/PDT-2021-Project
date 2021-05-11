using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject
    {
        private Vector2 position;
        public Stat DealDamageModifier { get; }
        public Stat TakeDamageModifier { get; }
        public Stat TakeKnockbackModifier { get; }
        
        private GridManager gridManager;

        public GridObject(
            Vector2 position,
            Stat dealDamageModifier,
            Stat takeDamageModifier,
            Stat takeKnockbackModifier
        )
        {
            this.position = position;
            DealDamageModifier = dealDamageModifier;
            TakeDamageModifier = takeDamageModifier;
            TakeKnockbackModifier = takeKnockbackModifier;

            gridManager = ManagerLocator.Get<GridManager>();

            gridManager.AddGridObject(position, this);
        }

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
        
        public Vector2 GetGridPosition()
        {
            return gridManager.ConvertWorldSpaceToGridSpace(position);
        }
    }
}
