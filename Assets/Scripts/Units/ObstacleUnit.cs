using Managers;
using UnityEngine;

namespace GridObjects
{
    public class ObstacleUnit : GridObject
    {
        protected override void Start()
        {
            base.Start();
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            var coord = gridManager.ConvertPositionToCoordinate(transform.position);
            transform.position = gridManager.ConvertCoordinateToPosition(coord);
            gridManager.AddGridObject(coord, this);
        }
    }
}