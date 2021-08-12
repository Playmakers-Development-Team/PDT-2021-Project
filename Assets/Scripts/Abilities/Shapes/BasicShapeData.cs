using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Grid.GridObjects;
using Managers;
using UnityEngine;
using Utilities;

namespace Abilities.Shapes
{
    [CreateAssetMenu(fileName = "BasicShapeData", menuName = "ScriptableObjects/BasicShapeData",
        order = 0)]
    public class BasicShapeData : ScriptableObject, IShape
    {
        [Serializable]
        private struct ShapePart
        {
            public OrdinalDirectionMask direction;
            public List<Vector2Int> coordinates;
            [Tooltip("Rotates from North for cardinal directions and rotates from North-East for "
                     + "non-cardinal directions")]
            public bool autoRotate;
            [Tooltip("This will try to raycast on the grid towards the first found unit")]
            public bool isLineOfSight;
            public int lineOfSightRange;

            public bool CannotBeDirected => direction == OrdinalDirectionMask.None;

            public bool CanBeDirectedTo(OrdinalDirection toDirection) =>
                direction.Contains(toDirection);

            public bool CanAutoRotateTo(OrdinalDirection toDirection) => autoRotate 
                && ((toDirection.IsDiagonal() && direction == OrdinalDirectionMask.NorthEast)
                               || direction == OrdinalDirectionMask.North);
        }

        [SerializeField] private List<ShapePart> shapeParts;

        // TODO Incomplete feature, See todo in GetAffectedCoordinates().
        // TODO Also abstract the mouse to a vector offset.
        [Tooltip("This only works for shapes with no specific direction")]
        [SerializeField, HideInInspector]
        private bool shouldFollowMouse;

        private bool HasShapeParts => shapeParts.Count > 0;
        public bool IsDiagonalShape => HasShapeParts && shapeParts
            .Any(p => p.direction.HasDiagonal());
        public bool IsCardinalShape => !IsDiagonalShape;
        public bool HasNoDirection => HasShapeParts && shapeParts
            .All(p => p.direction == OrdinalDirectionMask.None);
        public bool IsLineOfSight => HasShapeParts && shapeParts.All(p => p.isLineOfSight);
        public bool ShouldShowLine => IsLineOfSight;

        /// <summary>
        /// Retrieve all tiles which is hit by this shape. Can be used for highlighting which tiles
        /// are hit.
        /// </summary>
        /// <param name="originCoordinate">The starting point of the shape, usually the Unit position.</param>
        /// <param name="direction">The vector relative to the unit in which the ability should be aimed towards.</param>
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate,
                                                                 ShapeDirection direction) =>
            GetAffectedCoordinates(originCoordinate, direction);

        [Obsolete]
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate,
                                                                 Vector2 directionVector) =>
            GetAffectedCoordinates(originCoordinate, ShapeDirection.FromIsometric(directionVector));

        /// <summary>
        /// Retrieve units that are hit by this shape.
        /// </summary>
        /// <param name="originCoordinate">The starting point of the shape, usually the Unit position.</param>
        /// <param name="direction">The vector relative to the unit in which the ability should be aimed towards.</param>
        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, ShapeDirection direction)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            IEnumerable<Vector2Int> coordinates =
                GetAffectedCoordinates(originCoordinate, direction);
            return coordinates.SelectMany(gridManager.GetGridObjectsByCoordinate);
        }
        
        [Obsolete]
        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 directionVector) => 
            GetTargets(originCoordinate, ShapeDirection.FromIsometric(directionVector));

        public IEnumerable<GridObject> GetTargetsInAllDirections(Vector2Int originCoordinate)
        {
            var targets = Enumerable.Empty<GridObject>();

            foreach (OrdinalDirection direction in Enum.GetValues(typeof(OrdinalDirection)))
            {
                ShapeDirection shapeDirection = ShapeDirection.FromOrdinalDirection(direction);
                targets = targets.Concat(GetTargets(originCoordinate, shapeDirection));
            }

            return targets;
        }

        /// <summary>
        /// Get all world space coordinates that will be targeted by this shape based on the shape
        /// parts. 
        /// </summary>
        /// <param name="originCoordinate">The starting point of the shape, usually the Unit position.</param>
        /// <param name="shapeDirection">The vector relative to the unit in which the ability should be aimed towards.</param>
        private IEnumerable<Vector2Int> GetAffectedCoordinates(Vector2Int originCoordinate, 
                                                               ShapeDirection shapeDirection)
        {
            // We want to convert to cardinal direction first because we don't want to get the ordinal
            // directions e.g NorthEast. Only return the cardinal directions based on the vector.
            OrdinalDirection direction = IsDiagonalShape
                ? shapeDirection.OrdinalDirection
                : shapeDirection.CardinalDirection.ToOrdinalDirection();

            IEnumerable<Vector2Int> affectedCoordinates = shapeParts
                .Where(shapePart => shapePart.CannotBeDirected 
                            || shapePart.CanAutoRotateTo(direction) 
                            || shapePart.CanBeDirectedTo(direction))
                .SelectMany(shapePart =>
                {
                    IEnumerable<Vector2Int> coordinates = shapePart.coordinates;

                    // Check if we should perform the mechanism that automatically does rotation
                    // transformation (in 90 degree angles) for the shape
                    if (shapePart.autoRotate)
                    {
                        // Check if the direction that this shape is being used towards is diagonal
                        if (direction.IsDiagonal())
                        {
                            // We want to rotate the shape coordinates by the angle derived
                            // from the target vector.
                            // For diagonals, it will still be a rotation transformation
                            // in 90 degree angles, so we can use a trick to start back at north
                            // and switch to cardinal directions for the math.
                            coordinates = coordinates.Select(c =>
                                CardinalDirectionUtility.RotateVector2Int(c,
                                    CardinalDirection.North,
                                    direction.RotateAntiClockwise().ToCardinalDirection()));
                        }
                        else
                        {
                            // We want to rotate the shape coordinates by the angle derived
                            // from the target vector.
                            coordinates = coordinates.Select(c =>
                                CardinalDirectionUtility.RotateVector2Int(c,
                                    CardinalDirection.North, 
                                    direction.ToCardinalDirection()));
                        }
                    }

                    if (shapePart.isLineOfSight)
                    {
                        GridManager gridManager = ManagerLocator.Get<GridManager>();
                        Vector2Int offset = direction.ToVector2Int();
                        var gridObjects = gridManager.GridLineCast(originCoordinate, 
                            direction, shapePart.lineOfSightRange - 1);

                        // Check if we have hit anything
                        if (gridObjects.Any())
                        {
                            Vector2Int hitVector = 
                                gridObjects.First().Coordinate - originCoordinate;
                            coordinates = coordinates.Select(c => c + hitVector - offset);
                        }
                        // Otherwise, get the furthest tile that this shape can reach
                        else
                        {
                            coordinates = coordinates.Select(c =>
                                c + offset * (shapePart.lineOfSightRange - 1));
                        }
                    }

                    // Offset so that the shape is starting from the origin coordinate
                    return coordinates.Select(c => c + originCoordinate);
                });

            // TODO Incomplete, shove in mouse position via target vector and implement some sort of
            // TODO limit, at this point you might want to make this a different type of shape.
            // Handle the case where the shape can be placed anywhere in the world space via mouse
            if (HasNoDirection && shouldFollowMouse)
            {
                affectedCoordinates = affectedCoordinates.Select(c => c + originCoordinate);
            }

            return affectedCoordinates;
        }
    }
}
