using UnityEngine;

namespace Units
{
    public abstract class Unit : UnitBase<UnitDataBase>
    {
        public static IUnit Spawn<T1>(Vector2 worldPosition) where T1 : UnitDataBase
        {
            GameObject prefab = UnitSettings.GetPrefab<T1>();

            if (prefab is null)
            {
                Debug.LogError(
                    $"Prefab for Unit with data type {typeof(T1).Name} not assigned to UnitSettings" +
                    " ScriptableObject at Assets/Resources/Settings/UnitSettings.asset.");
                return null;
            }
            
            GameObject instance = Instantiate(prefab, worldPosition, Quaternion.identity);
            return instance.GetComponent<IUnit>();
        }

        public static IUnit Spawn<T1>(Vector2Int gridPosition) where T1 : UnitDataBase
        {
            // TODO: Uncomment once required functionality in GridManager has been implemented!
            // Vector2 worldPosition = GridManager.GridToWorld(gridPosition);
            Vector2 worldPosition = Vector2.one;
            return Spawn<T1>(worldPosition);
        }
    }
}
