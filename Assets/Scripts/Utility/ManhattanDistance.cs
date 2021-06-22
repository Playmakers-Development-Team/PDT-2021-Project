using UnityEngine;

namespace Utility
{
    public static class ManhattanDistance
    {

        public static int GetManhattanDistance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        
       
    }
}
