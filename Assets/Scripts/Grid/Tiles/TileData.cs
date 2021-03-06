using System.Collections.Generic;
using Grid.GridObjects;
using UnityEngine.Tilemaps;

namespace Grid.Tiles
{
    public class TileData
    {
        private TileBase tile;
        public List<GridObject> GridObjects { get; } = new List<GridObject>();

        public TileBase Tile => tile;

        public TileData(TileBase tileBase)
        {
            tile = tileBase;
        }

        public void AddGridObjects(GridObject gridObject)
        {
            if (!GridObjects.Contains(gridObject))
            {
                GridObjects.Add(gridObject);
            }
        }
        public void RemoveGridObjects(GridObject gridObject) => GridObjects.Remove(gridObject);

        public void ClearGridObjects() => GridObjects.Clear();
    }
}
