using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
    [CustomGridBrush(true, false, false, "Colour Brush")]
    public class ColourBrush : GridBrushBase
    {
        [SerializeField] private bool white;
        
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
            Tile replacement = tileReference.GetTile(white ? TileType.Fill : TileType.Colour);
            
            if (replacement)
                map.SetTile(position, replacement);
        }
    }
}
