using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : HistoricalCommand
    {
        private Vector2Int targetCoords, currentCoords;
        private GridManager gridManager;
        
        public MoveCommand(IUnit unit, Vector2Int target) : base(unit)
        {
            gridManager = ManagerLocator.Get<GridManager>();
            targetCoords = target;
        }

        public override void Execute()
        {
            gridManager.MoveUnit(targetCoords, Unit);
        }

        public override void Undo()
        {
            // TODO
        }
    }
}
