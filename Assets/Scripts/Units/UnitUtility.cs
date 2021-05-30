﻿using Managers;
using UnityEngine;

namespace Units
{
    public static class UnitUtility
    {
        public static IUnit Spawn(GameObject prefab, Vector2Int coordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Vector2 position = gridManager.ConvertCoordinateToPosition(coordinate);

            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            IUnit IUnit = instance.GetComponent<IUnit>();
            
            
            return IUnit;
        }

        public static IUnit Spawn(string unitName, Vector2Int coordinate)
        {
            GameObject prefab = UnitSettings.GetPrefab(unitName);
            return Spawn(prefab, coordinate);
        }
    }
}
