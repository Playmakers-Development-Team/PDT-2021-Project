using System;
using E7.Minefield;
using Grid;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tests.Beacons
{
    [ExecuteAlways]
    public class TileBeacon<T> : ScreenBeacon<T> where T : Enum
    {
        [SerializeField] private Vector2Int coordinate;
        [SerializeField] private bool autoSetTilemap = true;
        [SerializeField] private Tilemap levelTilemap;

        public Vector3 SnappedPosition => levelTilemap.layoutGrid.GetCellCenterWorld((Vector3Int) coordinate);

        private void Update()
        {
            if (transform.hasChanged && levelTilemap != null)
            {
                coordinate = (Vector2Int) levelTilemap.layoutGrid.WorldToCell(transform.position);
                transform.position = SnappedPosition;
            }
        }

        private void OnValidate()
        {
            if (!autoSetTilemap)
                return;
            
            if (levelTilemap == null)
            {
                GameObject go = GameObject.FindWithTag("LevelTilemap");
                levelTilemap = go.GetComponent<Tilemap>();
            }

            transform.position = SnappedPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position, new Vector3(0.3f, 0.3f, 0.1f));
        }
    }
}
