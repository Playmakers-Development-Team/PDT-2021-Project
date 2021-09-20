using System;
using System.Collections.Generic;
using Grid.GridObjects;
using UnityEngine;

namespace Abilities.Shapes
{
    public interface IShape
    {
        /// <summary>
        /// Should there be a line visual connecting all the affected tiles.
        /// Abilities such as Line of sight typically want this to be the case.
        /// </summary>
        /// <returns>True if the line visual should be shown</returns>
        public bool ShouldShowLine { get; }

        /// <summary>
        /// Is there a particular direction that players can use the mouse to aim towards? Some
        /// abilities can be aimed with the mouse while others can't - they only affect nearby units
        /// </summary>
        /// <returns>True, if the shape cannot be aimed with the mouse</returns>
        public bool HasNoDirection { get; }

        /// <summary>
        /// Check if the shape can affect units that are diagonal to the current unit
        /// </summary>
        /// <returns>True, if the shape can affect units diagonally</returns>
        public bool IsDiagonalShape { get; }

        /// <summary>
        /// Check if the shape can affect units that are in the cardinal direction relative to
        /// the current unit
        /// </summary>
        /// <returns>True, if the shape can affect cardinal directions</returns>
        public bool IsCardinalShape { get; }

        /// <summary>
        /// Get all the cells within this shape. May be empty if this shape is not an AoE.
        /// </summary>
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate, 
                                                                 ShapeDirection direction);
        
        [Obsolete("Use GetHighlightedCoordinates(Vector2Int, ShapeDirection)")]
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate, 
                                                                 Vector2 directionVector);
        
        public List<Vector2Int> GetPossibleCoordinates(Vector2Int originCoordinate);

        /// <summary>
        /// Get all targets that would be affected by this shape.
        /// </summary>
        /// <param name="originCoordinate">The position where this shape is used from, typically the unit position</param>
        /// <param name="direction">The direction which this shape is used</param>
        /// <returns>All targets that are affected</returns>
        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, ShapeDirection direction);
        
        [Obsolete("Use GetTargets(Vector2Int, ShapeDirection)")]
        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 directionVector);

        /// <summary>
        /// Gets all possible targets from a position to all possible directions. Might be useful for AI things.
        /// </summary>
        /// <param name="originCoordinate">The position where this shape is used from, typically the unit position</param>
        public IEnumerable<GridObject> GetTargetsInAllDirections(Vector2Int originCoordinate);
    }
}
