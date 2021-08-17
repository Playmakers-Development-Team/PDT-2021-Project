using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tests.Beacons.Base
{
    public class TileBeacon<T> : ScreenBeacon<T>, ITileBeacon where T : Enum
    {
        [SerializeField] protected bool autoSetTilemap = true;
        [SerializeField] protected Tilemap levelTilemap;
        
        public virtual Vector2Int Coordinate => 
            (Vector2Int) levelTilemap.layoutGrid.WorldToCell(transform.position);
        public virtual Vector3 SnappedPosition => 
            levelTilemap.layoutGrid.GetCellCenterWorld((Vector3Int) Coordinate);

        protected virtual void OnValidate()
        {
            if (!autoSetTilemap || levelTilemap != null)
                return;

            GameObject go = GameObject.FindWithTag("LevelTilemap");
            levelTilemap = go.GetComponent<Tilemap>();
        }
    }
}
