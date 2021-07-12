using Commands;
using UnityEngine;

namespace Units.Commands
{
    /// <summary>
    /// Executed before a unit starts moving.
    /// </summary>
    public class StartMoveCommand : HistoricalCommand
    {
        public Vector2Int TargetCoords { get; }
        public Vector2Int StartCoords { get; }

        public StartMoveCommand(IUnit unit, Vector2Int target) : base(unit)
        {
            TargetCoords = target;
            StartCoords = unit.Coordinate;
        }

        public override void Execute() => Unit.MoveUnit(this);

        // TODO
        public override void Undo() {}
    }
}
