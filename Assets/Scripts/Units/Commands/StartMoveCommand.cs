using Commands;
using GridObjects;
using Managers;
using UnityEngine;

namespace Units.Commands
{
    /// <summary>
    /// Executed before a unit starts moving.
    /// </summary>
    public class StartMoveCommand : HistoricalCommand
    {
        private GridManager gridManager;
        
        public Vector2Int TargetCoords { get; }
        
        public Vector2Int StartCoords { get; }

        public StartMoveCommand(IUnit unit, Vector2Int target) : base(unit)
        {
            gridManager = ManagerLocator.Get<GridManager>();
            TargetCoords = target;
            StartCoords = ((GridObject)unit).Coordinate;
        }

        public override void Execute()
        {
            gridManager.MoveUnit(this);
        }

        public override void Undo()
        {
            // TODO
        }
    }
}
