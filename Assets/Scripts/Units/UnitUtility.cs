using Grid;
using Managers;
using UnityEngine;

namespace Units
{
    public static class UnitUtility
    {
        public static IUnit Spawn(GameObject prefab, Vector2Int coordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            if (gridManager.GetGridObjectsByCoordinate(coordinate).Count == 0)
            {
                Vector2 position = gridManager.ConvertCoordinateToPosition(coordinate);
                
                GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
                IUnit unit = instance.GetComponentInChildren<IUnit>();
                unit.gameObject.transform.position = gridManager.ConvertCoordinateToPosition(coordinate);
                return unit;
            }
            else
            {
                // Can change this later if we want to allow multiple grid objects on a tile
                Debug.LogWarning("Failed to spawn " + prefab +
                                 " at " + coordinate.x + ", " + coordinate.y +
                                 " due to tile being occupied.");
                return null;
            }
        }

        public static IUnit Spawn(string unitName, Vector2Int coordinate)
        {
            GameObject prefab = UnitSettings.GetPrefab(unitName);
            return Spawn(prefab, coordinate);
        }
    }
}
