using System.Collections.Generic;
using GridObjects;
using UnityEngine;

namespace Abilities.Shapes
{
    public interface IShape
    {
        /// <summary>
        /// Should there be a line visual connecting all the affected tiles.
        /// Abilities such as Line of sight typically want this to be the case.
        /// </summary>
        public bool ShouldShowLine { get; }
        
        /// <summary>
        /// Get all the cells within this shape. May be empty if this shape is not an AoE.
        /// </summary>
        public IEnumerable<Vector2Int> GetHighlightedCoordinates(Vector2Int originCoordinate, Vector2 targetVector);

        public IEnumerable<GridObject> GetTargets(Vector2Int originCoordinate, Vector2 targetVector);
    }
}
