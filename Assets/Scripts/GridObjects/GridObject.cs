using Managers;
using UnityEngine;

namespace GridObjects
{
    public class GridObject : MonoBehaviour
    {
      //  public ValueStat MovementActionPoints { get; protected set; }
        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);

        private GridManager gridManager;

        protected virtual void Start()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.AddGridObject(Coordinate, this);
            Debug.Log("chef");
        }
    }
}
