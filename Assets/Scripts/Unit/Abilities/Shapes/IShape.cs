using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Unit.Abilities.Shapes
{
    public interface IShape
    {
        /// <summary>
        /// Get all the cells within this shape. May be empty if this shape is not an AoE.
        /// </summary>
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate, Vector2 targetVector);

        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 targetVector);
    }
}
