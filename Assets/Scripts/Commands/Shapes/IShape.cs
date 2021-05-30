using System.Collections.Generic;
using GridObjects;
using Units;
using UnityEngine;
using Utility;

namespace Commands.Shapes
{
    public interface IShape
    {
        /// <summary>
        /// Get all the cells at which this shape will hit. If this is not an AoE shape, it may just
        /// be empty.
        /// </summary>
        public IEnumerable<Vector2Int> GetHighlightedCells(Vector2Int originCoordinate, Vector2 targetVector);

        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 targetVector);
    }
}
