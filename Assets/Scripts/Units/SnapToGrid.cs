using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units
{
    [ExecuteInEditMode]
    public class SnapToGrid : MonoBehaviour
    {
        [SerializeField] protected bool autoSetTilemap = true;
        [SerializeField] protected Tilemap levelTilemap;
        
        public virtual Vector3 WorldClickPosition => gameObject.transform.position;
        
        [SerializeField] private Vector2Int coordinate;

        public virtual Vector3 SnappedPosition => 
            levelTilemap.layoutGrid.GetCellCenterWorld((Vector3Int) coordinate);

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (transform.hasChanged && levelTilemap != null)
                {
                    coordinate =
                        (Vector2Int) levelTilemap.layoutGrid.WorldToCell(transform.position);
                    transform.position = SnappedPosition;
                }
            }
        }

        protected  void OnValidate()
        {
            if (!autoSetTilemap || levelTilemap != null)
                return;
            GameObject go = GameObject.FindWithTag("LevelTilemap");
            if (go != null)
                levelTilemap = go.GetComponent<Tilemap>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.4f,0.7f,0.45f);
            Gizmos.DrawSphere(transform.position, 0.08f);
        }
    }
}
