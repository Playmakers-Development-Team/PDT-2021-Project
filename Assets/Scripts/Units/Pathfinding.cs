using System;
using System.Collections.Generic;
using Grid;
using Managers;
using UnityEngine;
using Utilities;
using TileData = Grid.Tiles.TileData;

namespace Units
{
    public static class Pathfinding
    {
        public static Dictionary<Vector2Int, int> GetDistanceToAllCells(Vector2Int startingCoordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Dictionary<Vector2Int, TileData> tileDatas = gridManager.tileDatas;
        
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            string allegiance = "";

            if (tileDatas[startingCoordinate].GridObjects.Count > 0)
                allegiance = tileDatas[startingCoordinate].GridObjects[0].tag;

            // Add the starting coordinate to the queue
            coordinateQueue.Enqueue(startingCoordinate);
            int distance = 0;
            visited.Add(startingCoordinate, distance);
            
            // Loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];

                // Add neighbours of node to queue
                VisitDistanceToAllNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                
                coordinateQueue.Dequeue();
            }

            return visited;
        }
    
        public static void VisitNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance,
                                     Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Dictionary<Vector2Int, TileData> tileDatas = gridManager.tileDatas;
        
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                // OR ignore the check if allegiance is empty
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag) ||
                    IgnoreAllegiance(allegiance))
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
        
        /// <summary>
        /// Used to determine if the Unit allegiance matters in the VisitNode scenario. If allegiance
        /// does not matter, than occupied tiles will be returned through VisitNode.
        /// </summary>
        /// <param name="allegiance"></param>
        /// <returns>True if allegiance is empty, false if it is not</returns>
        private static bool IgnoreAllegiance(string allegiance)
        {
            if (allegiance.Equals(String.Empty))
                return true;
            return false;
        }

        public static void VisitDistanceToAllNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance,
                                                  Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Dictionary<Vector2Int, TileData> tileDatas = gridManager.tileDatas;
        
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
                }else if(tileDatas[node].GridObjects[0].tag.Equals("PlayerUnit"))
                {
                    if (!visited.ContainsKey(node))
                    {
                        visited.Add(node, distance + 1);
                    }
                }
            }
        }
    
        private static void VisitPathNode(Vector2Int node, Dictionary<Vector2Int, Vector2Int> visited,
                                          Queue<Vector2Int> coordinateQueue, IUnit iunit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Dictionary<Vector2Int, TileData> tileDatas = gridManager.tileDatas;
        
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 || 
                    iunit.GetType() == tileDatas[node].GridObjects[0].GetType())
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
        /// Returns an empty list if it cannot find a path
        /// </summary>
        public static List<Vector2Int> GetCellPath(Vector2Int startingCoordinate,
                                                   Vector2Int targetCoordinate, IUnit iunit)
        {
            var visited = new Dictionary<Vector2Int, Vector2Int>();
            var coordinateQueue = new Queue<Vector2Int>();
            bool targetWasFound = false;
        

            coordinateQueue.Enqueue(startingCoordinate);
            while (coordinateQueue.Count > 0)
            {
                var currentNode = coordinateQueue.Peek();

                VisitPathNode(currentNode + CardinalDirection.North.ToVector2Int(), visited,
                    coordinateQueue, iunit);
                VisitPathNode(currentNode + CardinalDirection.East.ToVector2Int(), visited,
                    coordinateQueue, iunit);
                VisitPathNode(currentNode + CardinalDirection.South.ToVector2Int(), visited,
                    coordinateQueue, iunit);
                VisitPathNode(currentNode + CardinalDirection.West.ToVector2Int(), visited,
                    coordinateQueue, iunit);

                if (visited.ContainsKey(targetCoordinate))
                {
                    targetWasFound = true;
                    coordinateQueue.Clear();
                }
                else
                    coordinateQueue.Dequeue();
            }

            foreach (KeyValuePair<Vector2Int, Vector2Int> node in visited)
            {
                //Debug.Log($"NodeStuff {node}");
            }

            if (!targetWasFound)
            {
                return new List<Vector2Int>();
            }
        
            var path = new List<Vector2Int>();
            var currentNode2 = targetCoordinate;
            int count = 0;
            while (count < 20)
            {
                path.Add(currentNode2);
                if (visited.ContainsKey(currentNode2))
                    currentNode2 = visited[currentNode2];
                else
                    break;

                count++;
            }

            path.Reverse();
            return path;
        }
    }
}
