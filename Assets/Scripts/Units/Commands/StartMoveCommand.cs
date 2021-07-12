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
        private IUnit Unit;
        
        public Vector2Int TargetCoords { get; }
        
        public Vector2Int StartCoords { get; }

        public StartMoveCommand(IUnit unit, Vector2Int target) : base(unit)
        {
            Unit = unit;
            TargetCoords = target;
            StartCoords = unit.Coordinate;
        }

        public override void Execute()
        {
            Unit.MoveUnit(this);
        }

        public override void Undo()
        {
            // TODO
        }
    }
}
