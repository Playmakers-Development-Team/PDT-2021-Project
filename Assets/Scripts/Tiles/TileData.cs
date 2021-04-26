using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles
{
    public class TileData
    {
        private TileBase tile;
        private List<GameObject> gridObjects;
        
        public TileBase Tile => tile;

        public TileData(TileBase tileBase)
        {
            tile = tileBase;
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
