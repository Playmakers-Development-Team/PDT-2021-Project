using Managers;
using Units;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        private int HealthPoints { get; set; }
        private int MovementActionPoints { get; set; }
        public ModifierStat TakeDamageModifier { get; protected set; }
        public ModifierStat TakeKnockbackModifier { get; protected set; }
        
        private Vector2Int coordinate;
        
        private GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            coordinate = gridManager.ConvertPositionToCoordinate(transform.position);

            gridManager.AddGridObject(coordinate, this);
        }

        public void TakeDamage(int amount)
        {
            int damageTaken = (int) TakeDamageModifier.Modify(amount);
            HealthPoints -= damageTaken;
            Debug.Log(damageTaken + " damage taken.");
            Debug.Log($"Health Before: {HealthPoints + damageTaken}  |  Health After: {HealthPoints}");
            CheckDeath();
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }

        public Vector2Int GetCoordinate()
        {
            return coordinate;
        }

        private void CheckDeath()
        {
            if (HealthPoints <= 0)
                KillGridObject();
        }

        private void KillGridObject()
        {
            Debug.Log($"This Grid Object was cringe and died");
            
            gridManager.RemoveGridObject(coordinate, this);

            IUnit unit = (IUnit) this;
            
            ManagerLocator.Get<TurnManager>().RemoveUnitFromQueue(unit);

            if (unit is PlayerUnit)
            {
                ManagerLocator.Get<PlayerManager>().RemovePlayerUnit(unit);
            }
            else if (unit is EnemyUnit)
            {
                ManagerLocator.Get<EnemyManager>().RemoveEnemyUnit(unit);
            }
            else
            {
                Debug.LogError("ERROR: Failed to kill " + this.gameObject + 
                               " as it is an unidentified unit");
            }
            
            // "Delete" the gridObject (setting it to inactive just in case we still need it)
            gameObject.SetActive(false);
        }
    }
}
