using UnityEngine;

namespace Units
{
    public static class UnitUtility
    {
        public static Unit Spawn(GameObject prefab, Vector2Int coordinate)
        {
            // TODO: Uncomment once required functionality in GridManager has been implemented!
            // Vector2 position = GridManager.GridToWorld(coordinate);
            Vector2 position = coordinate;
            
            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            Unit unit = instance.GetComponent<Unit>();
            
            // GridManager.Occupy(unit);
            
            return unit;
        }

        public static Unit Spawn(string unitName, Vector2Int coordinate)
        {
            GameObject prefab = UnitSettings.GetPrefab(unitName);
            return Spawn(prefab, coordinate);
        }
    }
}
