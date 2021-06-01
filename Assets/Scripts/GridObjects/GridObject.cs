using System;
using Managers;
using Units;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
        public ValueStat HealthPoints { get; protected set; }
        public ValueStat MovementActionPoints { get; protected set; }
        public ModifierStat TakeDamageModifier { get; protected set; }
        public ModifierStat TakeKnockbackModifier { get; protected set; }
        
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
            HealthPoints.Value -= damageTaken;
            Debug.Log(damageTaken + " damage taken.");
            Debug.Log($"Health Before: {HealthPoints.Value + damageTaken}  |  Health After: {HealthPoints.Value}");
            CheckDeath();
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }

        [Obsolete("Use Coordinate property instead.")]
        public Vector2Int GetCoordinate()
        {
            return Coordinate;
        }

        private void CheckDeath()
        {
            if (HealthPoints.Value <= 0)
                KillGridObject();
        }

        private void KillGridObject()
        {
            Debug.Log($"This Grid Object was cringe and died");
            
            gridManager.RemoveGridObject(Coordinate, this);

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
