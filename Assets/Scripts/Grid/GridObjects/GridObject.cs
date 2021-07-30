using Commands;
using Grid.Commands;
using Managers;
using UnityEngine;

namespace Grid.GridObjects
{
    public class GridObject : MonoBehaviour
    {
        //  public ValueStat MovementActionPoints { get; protected set; }
        protected GridManager gridManager;
        protected CommandManager commandManager;

        public Vector2Int Coordinate => gridManager.ConvertPositionToCoordinate(transform.position);

        protected virtual void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected virtual void OnEnable() => commandManager.ListenCommand<GridReadyCommand>(OnGridReady);

        protected virtual void OnDisable() => commandManager.UnlistenCommand<GridReadyCommand>(OnGridReady);

        private void OnGridReady(GridReadyCommand obj)
        {
            gridManager.AddGridObject(Coordinate, this);

            // Snap objects to grid
            var coord = gridManager.ConvertPositionToCoordinate(transform.position);
            transform.position = gridManager.ConvertCoordinateToPosition(coord);
        }
    }
}
