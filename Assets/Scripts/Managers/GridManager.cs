using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using Units;
using Units.Commands;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : Manager
    {
        private Dictionary<Vector2Int, TileData> tileDatas = new Dictionary<Vector2Int, TileData>();

        private const int gridLineCastDefaultLimit = 10;

        public Tilemap LevelTilemap { get; private set; }
        public Vector2Int LevelBounds { get; private set; }
        public BoundsInt LevelBoundsInt { get; private set; }

        public void InitialiseGrid(Tilemap levelTilemap, Vector2Int levelBounds)
        {
            LevelBounds = levelBounds;
            LevelTilemap = levelTilemap;
            
            LevelBoundsInt = new BoundsInt(
                new Vector3Int(-Mathf.FloorToInt(levelBounds.x / 2.0f), -Mathf.FloorToInt(levelBounds.y / 2.0f), 0),
                new Vector3Int(levelBounds.x - 1, levelBounds.y - 1, 0)
            );

            for (int x = LevelBoundsInt.xMin; x <= LevelBoundsInt.xMax; x++)
            {
                for (int y = LevelBoundsInt.xMin; y <= LevelBoundsInt.yMax; y++)
                {
                    TileBase tile = levelTilemap.GetTile(new Vector3Int(x, y, 0));
                    // This is going to be null, if there is no tile there but that's fine
                    tileDatas.Add(new Vector2Int(x, y), new TileData(tile));

                    // Debug.Log($"Register tiledata at {x}, {y}");
                    //
                    // if (tile != null)
                    // {
                    //     tileDatas.Add(new Vector2Int(x, y), new TileData(tile));
                    // }
                }
            }
        }
        
        #region GETTERS

        public bool IsInBounds(Vector2Int coordinate)
        {
            return coordinate.x >= LevelBoundsInt.xMin && coordinate.x <= LevelBoundsInt.xMax &&
                   coordinate.y >= LevelBoundsInt.yMin && coordinate.y <= LevelBoundsInt.yMax;
        }

        public bool IsInBounds(Vector3 worldPosition, bool clamp = false)
        {
            Vector2Int coordinate = ConvertPositionToCoordinate(worldPosition, clamp);
            return IsInBounds(coordinate);
        }

        public TileData GetTileDataByCoordinate(Vector2Int coordinate)
        {
            if (tileDatas.TryGetValue(coordinate, out TileData tileData))
                return tileData;

            Debug.Log("No tile found at coordinates " + coordinate);
            return null;
        }

        public List<GridObject> GetGridObjectsByCoordinate(Vector2Int coordinate)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData is null)
            {
                Debug.Log("No tile found at coordinates " + coordinate);
                return new List<GridObject>();
            }

            return tileData.GridObjects;
        }

        public Vector2Int GetRandomCoordinates()
        {
            return new Vector2Int(Random.Range(-(LevelBounds.x / 2), (LevelBounds.x / 2)),
                Random.Range(-(LevelBounds.y / 2), (LevelBounds.y / 2)));
        }

        public Vector2Int GetRandomUnoccupiedCoordinates()
        {
            Vector2Int coordinate = GetRandomCoordinates();

            while (GetGridObjectsByCoordinate(coordinate).Count > 0)
            {
                coordinate = GetRandomCoordinates();
            }

            return coordinate;
        }

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<GridObject> GridLineCast(Vector2Int originCoordinate, Vector2 targetVector,
                                             int limit = gridLineCastDefaultLimit) =>
            GridLineCast(originCoordinate, OrdinalDirectionUtility.From(Vector2.up, targetVector));

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, Vector2 targetVector,
                                       int limit = gridLineCastDefaultLimit) where T : GridObject =>
            GridLineCast<T>(originCoordinate,
                OrdinalDirectionUtility.From(Vector2.up, targetVector), limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<GridObject> GridLineCast(Vector2Int originCoordinate,
                                             OrdinalDirection direction,
                                             int limit = gridLineCastDefaultLimit) =>
            GridLineCast<GridObject>(originCoordinate, direction, limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, OrdinalDirection direction,
                                       int limit = gridLineCastDefaultLimit) where T : GridObject
        {
            Vector2Int increment = direction.ToVector2Int();
            Vector2Int currentCoordinate = originCoordinate;

            for (int i = 0; i < limit; i++)
            {
                currentCoordinate += increment;
                var gridObjects = GetGridObjectsByCoordinate(currentCoordinate);

                if (gridObjects.Count > 0)
                {
                    var foundObjects = gridObjects.OfType<T>().ToList();

                    if (foundObjects.Any())
                        return foundObjects;
                }
            }

            return new List<T>();
        }

        /// <summary>
        /// Returns a list of all coordinates that are reachable from a given starting position
        /// within the given range.
        /// </summary>
        /// <param name="startingCoordinate">The coordinate to begin the search from.</param>
        /// <param name="range">The range from the starting tile using manhattan distance.</param>
        /// <returns>A list of the coordinates of reachable tiles.</returns>
        public List<Vector2Int> GetAllReachableTiles(Vector2Int startingCoordinate, int range)
        {
            List<Vector2Int> reachable = new List<Vector2Int>();
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            string allegiance = tileDatas[startingCoordinate].GridObjects[0].tag;
            
            // Add the starting coordinate to the queue
            coordinateQueue.Enqueue(startingCoordinate);
            int distance = 0;
            visited.Add(startingCoordinate, distance);

            // Loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];

                if (distance > range)
                {
                    break;
                }

                // Add neighbours of node to queue
                VisitNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);

                if (GetGridObjectsByCoordinate(currentNode).Count == 0)
                    reachable.Add(currentNode);

                coordinateQueue.Dequeue();
            }

            return reachable;
        }

        private void VisitNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance,
                               Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag))
                {
                    // Check node has not already been visited
                    if (!visited.ContainsKey(node))
                    {
                        // Add node to queue and store the distance taken to arrive at it
                        visited.Add(node, distance + 1);
                        coordinateQueue.Enqueue(node);
                    }
                }
            }
        }

        private void VisitNode(Vector2Int node, Dictionary<Vector2Int, Vector2Int> visited,
                               Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag))
                {
                    // Check node has not already been visited
                    if (!visited.ContainsKey(node) && !visited.ContainsValue(node))
                    {
                        // Add node to queue and store the previous node
                        visited.Add(node, coordinateQueue.Peek());
                        coordinateQueue.Enqueue(node);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of the path from one node to another
        /// Assumes target is reachable.
        /// </summary>
        private List<Vector2Int> GetCellPath(Vector2Int startingCoordinate,
                                             Vector2Int targetCoordinate)
        {
            var visited = new Dictionary<Vector2Int, Vector2Int>();
            var coordinateQueue = new Queue<Vector2Int>();
            string allegiance = GetTileDataByCoordinate(startingCoordinate).GridObjects[0].
                gameObject.tag;

            coordinateQueue.Enqueue(startingCoordinate);
            while (coordinateQueue.Count > 0)
            {
                var currentNode = coordinateQueue.Peek();

                VisitNode(currentNode + CardinalDirection.North.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.East.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.South.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitNode(currentNode + CardinalDirection.West.ToVector2Int(), visited,
                    coordinateQueue, allegiance);

                if (visited.ContainsKey(targetCoordinate))
                    coordinateQueue.Clear();
                else
                    coordinateQueue.Dequeue();
            }

            foreach (KeyValuePair<Vector2Int, Vector2Int> node in visited)
            {
                Debug.Log($"NodeStuff {node}");
            }

            var path = new List<Vector2Int>();
            var currentNode2 = targetCoordinate;
            while (true)
            {
                path.Add(currentNode2);
                if (visited.ContainsKey(currentNode2))
                    currentNode2 = visited[currentNode2];
                else
                    break;
            }

            path.Reverse();
            return path;
        }

        #endregion

        #region CONVERSIONS

        public Vector2Int ConvertPositionToCoordinate(Vector2 position, bool clamp = false)
        {
            Vector3Int unbounded = LevelTilemap.layoutGrid.WorldToCell(position);

            if (!clamp)
                return (Vector2Int) unbounded;
            
            return new Vector2Int(
                Mathf.Clamp(unbounded.x, LevelBoundsInt.xMin, LevelBoundsInt.xMax),
                Mathf.Clamp(unbounded.y, LevelBoundsInt.xMin, LevelBoundsInt.xMax)
                );
        }

        public Vector2 ConvertCoordinateToPosition(Vector2Int coordinate) => LevelTilemap.GetCellCenterWorld((Vector3Int) coordinate);

        #endregion

        #region GRID OBJECT FUNCTIONS

        public bool AddGridObject(Vector2Int coordinate, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData != null)
            {
                if (tileData.GridObjects.Count > 0)
                {
                    // Can change this later if we want to allow multiple grid objects on a tile
                    Debug.LogWarning("Failed to add " + gridObject + " at " + coordinate.x + ", " +
                                     coordinate.y + " due to tile being occupied by " +
                                     tileData.GridObjects[0]);
                    return false;
                }

                tileData.AddGridObjects(gridObject);
                Debug.Log(gridObject + " added to tile " + coordinate.x + ", " + coordinate.y);
                return true;
            }

            Debug.LogWarning("Failed to add " + gridObject + " at " + coordinate.x + ", " +
                             coordinate.y + " due to null tileData");

            return false;
        }

        public bool RemoveGridObject(Vector2Int coordinate, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData.GridObjects.Contains(gridObject))
            {
                // Debug.Log("GridObject removed from tile " + coordinate.x + ", " + coordinate.y);
                tileData.RemoveGridObjects(gridObject);
                return true;
            }

            Debug.LogWarning("Failed to remove gridObject at " + coordinate.x + ", " +
                             coordinate.y + ". Tile does not contain gridObject");

            return false;
        }

        public void MoveAllGridObjects(Vector2Int currentCoordinate, Vector2Int newCoordinate)
        {
            List<GridObject> gridObjects = GetGridObjectsByCoordinate(currentCoordinate);

            foreach (var gridObject in gridObjects)
            {
                MoveGridObject(currentCoordinate, newCoordinate, gridObject);
            }
        }
        
        // TODO: Move from Grid system to Unit system
        public async void MoveUnit(StartMoveCommand moveCommand)
        {
            IUnit unit = moveCommand.Unit;

            Vector2Int newCoordinate = moveCommand.TargetCoords;

            TileData tileData = GetTileDataByCoordinate(newCoordinate);
            if (tileData is null)
            {
                throw new Exception($"No tile data at coordinate {newCoordinate}. " +
                                    "Failed to move unit");
            }
            
            int moveRange = unit.MovementActionPoints.Value;
            Vector2Int startingCoordinate = unit.Coordinate;
            Vector2Int currentCoordinate = startingCoordinate;
            
            // Check if tile is unoccupied
            if (tileData.GridObjects.Count != 0)
            {
                // TODO: Provide feedback to the player
                Debug.Log("Target tile is occupied.");
                return;
            }

            // Check if tile is in range

            if (!GetAllReachableTiles(currentCoordinate, moveRange).Contains(newCoordinate) &&
                unit.GetType() == typeof(PlayerUnit))
            {
                // TODO: Provide feedback to the player
                
                Debug.Log("Target tile out of range.");
                return;
            }
            
            // TODO: Tween based on cell path
            List<Vector2Int> movePath = GetCellPath(currentCoordinate, newCoordinate);

            for (int i = 1; i < movePath.Count; i++)
            {
                   unit.UnitAnimator.SetBool("moving", true);
                        if (movePath[i].x > currentCoordinate.x)
                        unit.ChangeAnimation(AnimationStates.Right);
                    else if (movePath[i].y > currentCoordinate.y)
                        unit.ChangeAnimation(AnimationStates.Up);
                    else if (movePath[i].x < currentCoordinate.x)
                        unit.ChangeAnimation(AnimationStates.Left);
                    else if (movePath[i].y < currentCoordinate.y)
                        unit.ChangeAnimation(AnimationStates.Down);
                        
                await MovementTween(unit.gameObject, ConvertCoordinateToPosition(currentCoordinate),
                    ConvertCoordinateToPosition(movePath[i]), 1f);
                unit.gameObject.transform.position = ConvertCoordinateToPosition(movePath[i]);
                currentCoordinate = movePath[i];
            }

            MoveGridObject(startingCoordinate, newCoordinate, (GridObject) unit);
            unit.SetMovementActionPoints( - Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate)));
            
            unit.ChangeAnimation(AnimationStates.Idle);
            
            // Should be called when all the movement and tweening has been completed
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new EndMoveCommand(moveCommand));
        }

        private async UniTask MovementTween(GameObject unit, Vector3 startPos, Vector3 endPos,
                                            float duration)
        {
            float flag = 0f;
            Debug.Log("Tween unit from " + startPos + " to " + endPos + ".");
            while (flag < duration)
            {
                unit.transform.position = Vector3.Lerp(startPos, endPos, flag / duration);
                flag += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// Moves a unit's GridObject and GameObject directly to a new position.
        /// </summary>
        /// <param name="newCoordinate">The coordinate to move the unit to.</param>
        /// <param name="unit">The unit to teleport.</param>
        private void TeleportUnit(Vector2Int newCoordinate, IUnit unit)
        {
            Vector2Int startCoordinate = unit.Coordinate;
            var gridObject = (GridObject) unit;

            gridObject.gameObject.transform.position = ConvertCoordinateToPosition(newCoordinate);

            MoveGridObject(startCoordinate, newCoordinate, gridObject);
        }

        // TODO: CurrentCoordinate should not be necessary
        public void MoveGridObject(Vector2Int currentCoordinate, Vector2Int newCoordinate,
                                   GridObject gridObject)
        {
            if (!AddGridObject(newCoordinate, gridObject))
                return;

            if (!RemoveGridObject(currentCoordinate, gridObject))
                return;

            Debug.LogFormat("Moved {0} from {1} to {2}.", gridObject, currentCoordinate,
                newCoordinate);
        }

        public List<GridObject> GetAdjacentGridObjects(Vector2Int coordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.up));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.right));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.down));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.left));

            return adjacentGridObjects;
        }

        #endregion
    }
}