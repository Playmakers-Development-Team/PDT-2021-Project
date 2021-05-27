using UnityEngine;

namespace Commands.Shapes
{
    public class Point : Shape
    {
        public Point(Vector2Int origin) : base(origin)
        {
            Cells.Add(Origin);
        }
    }
}
