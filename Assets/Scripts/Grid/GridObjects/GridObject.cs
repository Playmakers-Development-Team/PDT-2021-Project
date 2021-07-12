using Managers;
using UnityEngine;

namespace Grid.GridObjects
{
    public class GridObject : MonoBehaviour
    {
      //  public ValueStat MovementActionPoints { get; protected set; }
        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);

        protected GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.AddGridObject(Coordinate, this);
            
            // Snap objects to grid
            var coord = gridManager.ConvertPositionToCoordinate(transform.position);
            transform.position = gridManager.ConvertCoordinateToPosition(coord);
        }
    }
}
