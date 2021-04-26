using UnityEngine;

namespace Units
{
    public abstract class Unit : UnitBase<UnitDataBase>
    {
        public static IUnit Spawn(GameObject prefab, Vector2 position)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            return instance.GetComponent<IUnit>();
        }

        public static IUnit Spawn(GameObject prefab, Vector2Int coordinate)
        {
            // TODO: Replace line once required functionality in GridManager has been implemented!
            // Vector2 position = GridManager.GridToWorld(coordinate);
            Vector2 position = Vector2.one;
            return Spawn(prefab, position);
        }

        public static IUnit Spawn(string unitName, Vector2 position)
        {
            GameObject prefab = UnitSettings.GetPrefab(unitName);
            return Spawn(prefab, position);
        }

        public static IUnit Spawn(string unitName, Vector2Int coordinate)
        {
            GameObject prefab = UnitSettings.GetPrefab(unitName);
            return Spawn(prefab, coordinate);
        }
    }
}
