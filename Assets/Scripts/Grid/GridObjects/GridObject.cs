using System;
using Managers;
using UnityEngine;

namespace Grid.GridObjects
{
    public class GridObject : MonoBehaviour
    {
      //  public ValueStat MovementActionPoints { get; protected set; }
      protected GridManager gridManager;
        
        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);

        protected virtual void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            
            gridManager.AddGridObject(Coordinate, this);
            
            // Snap objects to grid
            var coord = gridManager.ConvertPositionToCoordinate(transform.position);
            transform.position = gridManager.ConvertCoordinateToPosition(coord);
        }
    }
}
