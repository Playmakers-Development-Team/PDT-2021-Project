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
    [CustomGridBrush(true, false, false, "Colour Brush")]
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

            TileReference tileReference = references.Find(reference => reference.HasTile(current as Tile));
            Tile replacement = tileReference.GetTile(type);
            
            if (replacement)
                map.SetTile(position, replacement);
        }
#endif
    }
}
