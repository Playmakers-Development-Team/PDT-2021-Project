using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Units;
using UnityEngine;
using Utility;

namespace Commands.Shapes
{
    [CreateAssetMenu(fileName = "BasicShapeData", menuName = "ScriptableObjects/ShapeData", order = 0)]
    public class BasicShapeData : ScriptableObject, IShape
    {
        [Serializable]
        private struct ShapePart
        {
            public OrdinalDirectionMask direction;
            public List<Vector2Int> coordinates;
            [Tooltip("This only works for cardinal directions")]
            public bool autoRotateFromNorth;
            [Tooltip("This will try to raycast on the grid towards the first found unit")]
            public bool isLineOfSight;
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

        public IEnumerable<IUnit> GetTargets(Vector2Int originCoordinate, Vector2 targetVector)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            IEnumerable<Vector2Int> coordinates = GetAffectedCoordinates(originCoordinate, targetVector);

            throw new NotImplementedException();
            // TODO how do you get a unit from a coordinate (a.k.a grid position)?
            //return cells.Select(coor => ?????);
        }

        private IEnumerable<Vector2Int> GetAffectedCoordinates(Vector2Int originCoordinate, Vector2 targetVector)
        {
            bool isDiagonalShape = IsDiagonalShape;

            // We want to convert to cardinal direction first because we don't want to get the ordinal
            // directions e.g NorthEast. Only return the cardinal directions based on the vector.
            OrdinalDirection direction = isDiagonalShape
                ? OrdinalDirectionUtility.From(Vector2.zero, targetVector)
                : CardinalDirectionUtility.From(Vector2.zero, targetVector).ToOrdinalDirection();

            IEnumerable<Vector2Int> affectedCoordinates = shapeParts.
                Where(p => p.direction.Contains(direction)).SelectMany(p =>
                {
                    IEnumerable<Vector2Int> coordinates = p.coordinates;

                    if (p.autoRotateFromNorth && isDiagonalShape)
                    {
                        coordinates = coordinates.Select(c =>
                            CardinalDirectionUtility.RotateVector2Int(c, CardinalDirection.North,
                                direction.ToCardinalDirection()));
                    }

                    if (p.isLineOfSight)
                    {
                        // TODO somehow do linecasting or raycasting from somewhere relative to origin
                        Vector2Int hitVector = Vector2Int.zero;

                        coordinates = coordinates.Select(c => c + hitVector);
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
