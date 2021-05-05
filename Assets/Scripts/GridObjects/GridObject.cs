using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject
    {
        private Vector2Int position;
        private ITakeDamageBehaviour takeDamageBehaviour;
        private ITakeKnockbackBehaviour takeKnockbackBehaviour;
        
        private GridManager gridManager;

        public GridObject(
            Vector2Int position,
            ITakeDamageBehaviour takeDamageBehaviour,
            ITakeKnockbackBehaviour takeKnockbackBehaviour
        )
        {
            this.position = position;
            this.takeDamageBehaviour = takeDamageBehaviour;
            this.takeKnockbackBehaviour = takeKnockbackBehaviour;

            gridManager = ManagerLocator.Get<GridManager>();

            gridManager.AddGridObject(position, this);
        }

        public void TakeDamage(int amount)
        {
            int damageTaken = takeDamageBehaviour.TakeDamage(amount);
            Debug.Log(damageTaken + " damage taken.");
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = takeKnockbackBehaviour.TakeKnockback(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }
    }
}
