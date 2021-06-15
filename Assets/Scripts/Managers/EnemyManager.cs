using System.Collections.Generic;
using GridObjects;
using Units;
using UnityEngine;

namespace Managers
{
    
    public class EnemyManager: Manager
    {
        
       
        // IsPlayerAdjacent will return true as soon as it finds a player adjacent to the given gridObject
        // otherwise will return false
        public GridObject FindAdjacentPlayer(GridObject gridObject)
        {
            Vector2Int gridObjectPosition = gridObject.Coordinate;
            
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.up));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.right));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.down));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.left));

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                {
                    return adjacentGridObject;
                }
            }
            
            return null;
        }
    }
}
