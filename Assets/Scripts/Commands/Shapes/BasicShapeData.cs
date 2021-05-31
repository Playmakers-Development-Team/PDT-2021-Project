using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using Managers;
using Units;
using UnityEngine;
using Utility;

namespace Commands.Shapes
{
    [CreateAssetMenu(fileName = "BasicShapeData", menuName = "ScriptableObjects/BasicShapeData", order = 0)]
    public class BasicShapeData : ScriptableObject, IShape
    {
        [Serializable]
        private struct ShapePart
        {
            public OrdinalDirectionMask direction;
            public List<Vector2Int> coordinates;
            [Tooltip("This only works for cardinal directions. Rotates from North for cardinal directions and rotates from North-East for non-cardinal directions")]
            public bool autoRotate;
            [Tooltip("This will try to raycast on the grid towards the first found unit")]
            public bool isLineOfSight;
            public int lineOfSightRange;
        }

        [SerializeField] private List<ShapePart> shapeParts;

        [Tooltip("This only works for shapes with no specific direction")]
        [SerializeField, HideInInspector]
        private bool shouldFollowMouse;

        public bool IsDiagonalShape => shapeParts.Any(p => p.direction.HasDiagonal());

        public bool HasNoDirection => shapeParts.All(p => p.direction == OrdinalDirectionMask.None);

        public bool IsLineOfSight => shapeParts.All(p => p.isLineOfSight);

        public IEnumerable<Vector2Int> GetHighlightedCells(Vector2Int originCoordinate, Vector2 targetVector) =>
            GetAffectedCoordinates(originCoordinate, targetVector);

        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 targetVector)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            IEnumerable<Vector2Int> coordinates = GetAffectedCoordinates(originCoordinate, targetVector);
            return coordinates.SelectMany(gridManager.GetGridObjectsByCoordinate);
        }

        private IEnumerable<Vector2Int> GetAffectedCoordinates(Vector2Int originCoordinate, Vector2 targetVector)
        {
            // We want to convert to cardinal direction first because we don't want to get the ordinal
            // directions e.g NorthEast. Only return the cardinal directions based on the vector.
            OrdinalDirection direction = IsDiagonalShape
                ? OrdinalDirectionUtility.From(Vector2.zero, targetVector)
                : CardinalDirectionUtility.From(Vector2.zero, targetVector).ToOrdinalDirection();

            IEnumerable<Vector2Int> affectedCoordinates = shapeParts.
                Where(p => p.direction.Contains(direction)).SelectMany(p =>
                {
                    IEnumerable<Vector2Int> coordinates = p.coordinates;

                    if (p.autoRotate)
                    {
                        if (direction.IsDiagonal())
                        {
                            coordinates = coordinates.Select(c =>
                                CardinalDirectionUtility.RotateVector2Int(c, CardinalDirection.North,
                                    direction.RotateAntiClockwise().ToCardinalDirection()));
                        }
                        else
                        {
                            coordinates = coordinates.Select(c =>
                                CardinalDirectionUtility.RotateVector2Int(c, CardinalDirection.North,
                                    direction.ToCardinalDirection()));
                        }
                    }

                    if (p.isLineOfSight)
                    {
                        GridManager gridManager = ManagerLocator.Get<GridManager>();
                        Vector2Int offset = direction.ToVector2Int();
                        var gridObjects = gridManager.GridLineCast(originCoordinate, direction, p.lineOfSightRange);

                        if (gridObjects.Any())
                        {
                            Vector2Int hitVector = gridObjects.First().Coordinate - originCoordinate;
                            coordinates = coordinates.Select(c => c + hitVector - offset);
                        }
                    }

                    return coordinates;
                });

            if (HasNoDirection && shouldFollowMouse)
            {
                affectedCoordinates = affectedCoordinates.Select(c => c + originCoordinate);
            }

            return affectedCoordinates;
        }
    }
}