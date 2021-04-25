using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    public class TileData
    {
        private TileBase tile;
        private Vector2 position;
        
        public TileBase Tile => tile;
        public Vector2 Position => position;
        
        public TileData(TileBase tileBase, Vector2 tilePosition)
        {
            tile = tileBase;
            position = tilePosition;
        }
    }
}
