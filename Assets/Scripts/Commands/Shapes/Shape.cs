using System.Collections.Generic;
using UnityEngine;

namespace Commands.Shapes
{
    public abstract class Shape
    {
        public Vector2Int Origin { get; }
        public List<Vector2Int> Cells { get; } = new List<Vector2Int>();

        public Shape(Vector2Int origin)
        {
            Origin = origin;
        }
    }
}
