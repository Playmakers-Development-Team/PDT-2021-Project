using UnityEngine;

namespace GridObjects
{
    public class GridObject
    {
        private ITakeDamageBehaviour takeDamageBehaviour;
        private ITakeKnockbackBehaviour takeKnockbackBehaviour;
        
        public GridObject(
            ITakeDamageBehaviour takeDamageBehaviour,
            ITakeKnockbackBehaviour takeKnockbackBehaviour
        )
        {
            this.takeDamageBehaviour = takeDamageBehaviour;
            this.takeKnockbackBehaviour = takeKnockbackBehaviour;
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
