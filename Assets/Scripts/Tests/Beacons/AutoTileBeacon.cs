using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tests.Beacons
{
    [ExecuteAlways]
    public class AutoTileBeacon<T> : TileBeacon<T>, ITileBeacon where T : Enum
    {
        [SerializeField] private Vector2Int coordinate;

        public override Vector2Int Coordinate => coordinate;

        private void Update()
        {
            if (transform.hasChanged && levelTilemap != null)
            {
                coordinate = (Vector2Int) levelTilemap.layoutGrid.WorldToCell(transform.position);
                transform.position = SnappedPosition;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            transform.position = SnappedPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position, new Vector3(0.3f, 0.3f, 0.1f));
        }
    }
}
