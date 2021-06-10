using System;
using System.Collections.Generic;
using GridObjects;
using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : HistoricalCommand
    {
        public IUnit Unit { get; }
        private Vector2Int targetCoords, currentCoords;
        private GridManager gridManager;
        
        public MoveCommand(IUnit unit, Vector2Int target, Vector2Int current)
        {
            Unit = unit;
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
