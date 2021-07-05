using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background.Tiles
{
    /// <summary>
    /// Substitutes tiles for the specified <see cref="TileType"/> variant.
    /// </summary>
    [CustomGridBrush(true, false, false, "Substitute Brush")]
    public class SubstituteBrush : GridBrushBase
    {
        [SerializeField] private TileType type;
        
#if UNITY_EDITOR
        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            List<TileReference> references = new List<TileReference>();
            string[] guids = AssetDatabase.FindAssets("t: TileReference");
            foreach (string guid in guids)
            {
                TileReference reference = AssetDatabase.LoadAssetAtPath<TileReference>(AssetDatabase.GUIDToAssetPath(guid));
                if (reference)
                    references.Add(reference);
            }
            
            Tilemap map = brushTarget.GetComponent<Tilemap>();
            TileBase current = map.GetTile(position);

            if (!current)
                return;

            TileReference tileReference = references.Find(reference => reference.HasTile((Tile) current));

            if (tileReference == null)
            {
                Debug.LogError(
                    "Attempting to substitute a tile which is not part of a tile reference." +
                    "Make sure the tile palette contains the tile processed by the tile creator."
                    );
                return;
            }
            
            Tile replacement = tileReference.GetTile(type);
            
            if (replacement)
                map.SetTile(position, replacement);
        }
#endif
    }
}
