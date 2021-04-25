using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    public class TileData
    {
        private TileBase tile;
        private Vector2 position;
        private List<GameObject> gridObjects;
        
        public TileBase Tile => tile;
        public Vector2 Position => position;
        
        public TileData(TileBase tileBase, Vector2 tilePosition)
        {
            tile = tileBase;
            position = tilePosition;
        }

        public void AddGridObjects(GameObject gameObject)
        {
            if (!gridObjects.Contains(gameObject))
            {
                gridObjects.Add(gameObject);
            }
        }
        
        public void RemoveGridObjects(GameObject gameObject)
        {
            gridObjects.Remove(gameObject);
        }
        
        public void ClearGridObjects()
        {
            gridObjects.Clear();
        }
    }
}
