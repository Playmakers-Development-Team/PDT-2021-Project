using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Units;
using UnityEngine;
using Utility;

namespace Commands.Shapes
{
    [CreateAssetMenu(fileName = "BasicShapeData", menuName = "ScriptableObjects/ShapeData", 
        order = 0)]
    public class BasicShapeData : ScriptableObject, IShape
    {
        [Serializable]
        private struct ShapePart
        {
            public OrdinalDirectionMask direction;
            public List<Vector2Int> coordinates;
            [Tooltip("This only works for cardinal directions")]
            public bool autoRotateFromNorth;
        }

        [SerializeField] private List<ShapePart> shapeParts;
        [Tooltip("This only works for shapes with no specific direction")]
        [SerializeField, HideInInspector] private bool shouldFollowMouse;

        public bool IsDiagonalShape => shapeParts.Any(p => p.direction.HasDiagonal());

        public bool HasNoDirection =>
            shapeParts.All(p => p.direction == OrdinalDirectionMask.None);

        public IEnumerable<Vector2Int> GetHighlightedCells(Vector2 targetVector) =>
            GetAffectedCoordinates(targetVector);

        public IEnumerable<IUnit> GetTargets(Vector2Int originCoordinate, Vector2 targetVector)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            IEnumerable<Vector2Int> coordinates = GetAffectedCoordinates(targetVector);

            if (HasNoDirection && shouldFollowMouse)
            {
                coordinates = coordinates.Select(c => c + originCoordinate);
            }

            throw new NotImplementedException();
            // TODO how do you get a unit from a coordinate (a.k.a grid position)?
            //return cells.Select(coor => ?????);
        }

        private IEnumerable<Vector2Int> GetAffectedCoordinates(Vector2 targetVector)
        {
            bool isDiagonalShape = IsDiagonalShape;

            // We want to convert to cardinal direction first because we don't want to get the ordinal
            // directions e.g NorthEast. Only return the cardinal directions based on the vector.
            OrdinalDirection direction = isDiagonalShape
                ? OrdinalDirectionUtility.From(Vector2.zero, targetVector)
                : CardinalDirectionUtility.From(Vector2.zero, targetVector).ToOrdinalDirection();

            return shapeParts.Where(p => p.direction.Contains(direction)).SelectMany(p =>
                p.autoRotateFromNorth && isDiagonalShape
                    ? p.coordinates.Select(c =>
                        CardinalDirectionUtility.RotateVector2Int(c, CardinalDirection.North,
                            direction.ToCardinalDirection()))
                    : p.coordinates);
        }
    }
}
