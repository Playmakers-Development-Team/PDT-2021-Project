using System.Collections.Generic;
using GridObjects;
using Units;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : UnitManager
    {
        /// <summary>
        /// Holds all the enemy units currently in the level.
        /// </summary>
        private readonly List<IUnit> enemyUnits = new List<IUnit>();

        /// <summary>
        /// Returns all enemy units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> EnemyUnits => enemyUnits.AsReadOnly();

        /// <summary>
        /// Clears all the enemies from the <c>enemyUnits</c> list.
        /// </summary>
        public void ClearEnemyUnits() => enemyUnits.Clear();

        /// <summary>
        /// Spawns in an enemy unit and adds it the the <c>enemyUnits</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c> that was added.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
            enemyUnits.Add(newUnit);
            return newUnit;
        }

        public GridObject FindAdjacentPlayer(GridObject gridObject)
        {
            Vector2Int gridObjectPosition = gridObject.Coordinate;

            GridManager gridManager = ManagerLocator.Get<GridManager>();

            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.up));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.right));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.down));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.left));

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                    return adjacentGridObject;
            }

            return null;
        }
    }
}
