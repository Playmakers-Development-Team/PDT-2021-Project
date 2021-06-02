using System;
using System.Collections.Generic;
using GridObjects;
using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : Command
    {
        private Vector2Int targetCoords, currentCoords;
        private GridManager gridManager;
        
        public MoveCommand(IUnit unit, Vector2Int target, Vector2Int current) : base(unit)
        {
            gridManager = ManagerLocator.Get<GridManager>();
            targetCoords = target;
            currentCoords = current;
        }

        public override void Queue() {}

        public override void Execute()
        {
            gridManager.MoveUnit(currentCoords, targetCoords, Unit);
        }

        public override void Undo() {}

        #region UnusedPathfinding
        // TODO: Review once fully implemented
        
        private void InitialiseGrid()
        {
            int width = 20;
            int height = 20;
            int[,] gridArray = new int[width, height];
            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    gridArray[i, j] = 0;
                }
            }

            gridArray = GetRange(gridArray, 5, new Vector2Int(6, 6));

            Print2DArray(gridArray);
        }

        private int[,] GetRange(int[,] gridArray, int moveRange, Vector2Int initialPos)
        {
            Queue<Vector2Int> coordQueue = new Queue<Vector2Int>();
            coordQueue.Enqueue(initialPos);

            while (coordQueue.Count > 0)
            {
                Vector2Int current = coordQueue.Peek();
                int currentMoveCount = gridArray[current.x, current.y];
                if (currentMoveCount == moveRange)
                {
                    coordQueue.Clear();
                    break;
                }

                //mark adjacent grids and add them to the back of the queue
                //Only mark if 0
                //Implement Method to increase maintainability
                if (gridArray[current.x + 1, current.y] == 0)
                {
                    gridArray[current.x + 1, current.y] = currentMoveCount + 1;
                    coordQueue.Enqueue(new Vector2Int(current.x + 1, current.y)); //right 
                }

                if (gridArray[current.x, current.y - 1] == 0)
                {
                    gridArray[current.x, current.y - 1] = currentMoveCount + 1;
                    coordQueue.Enqueue(new Vector2Int(current.x, current.y - 1)); //down
                }

                if (gridArray[current.x - 1, current.y] == 0)
                {
                    gridArray[current.x - 1, current.y] = currentMoveCount + 1;
                    coordQueue.Enqueue(new Vector2Int(current.x - 1, current.y)); //left
                }

                if (gridArray[current.x, current.y + 1] == 0)
                {
                    gridArray[current.x, current.y + 1] = currentMoveCount + 1;
                    coordQueue.Enqueue(new Vector2Int(current.x, current.y + 1)); //up 
                }

                coordQueue.Dequeue();
                //repeat until queue empty
            }

            return gridArray;
        }

        private void Print2DArray(int[,] gridArray)
        {
            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                Console.Write("\n[");
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    Console.Write(gridArray[j, i] + "", "");
                }

                Console.Write("]");
            }
        }
        
        #endregion
    }
}
