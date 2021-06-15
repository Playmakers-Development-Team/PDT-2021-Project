using Grid;
using Managers;
using UnityEngine;

namespace Unit.Commands
{
    public class MoveCommand : HistoricalCommand
    {
        private Vector2Int targetCoords, currentCoords;
        private GridManager gridManager;
        
        public MoveCommand(IUnit unit, Vector2Int target, Vector2Int current) : base(unit)
        {
            gridManager = ManagerLocator.Get<GridManager>();
            targetCoords = target;
            currentCoords = current;
        }

        public override void Execute()
        {
            gridManager.MoveUnit(currentCoords, targetCoords, Unit);
        }

        public override void Undo()
        {
            // TODO
        }
    }
}
