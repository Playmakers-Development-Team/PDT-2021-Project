using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
    public class PreviewMap : MonoBehaviour
    {
        [SerializeField] private Tilemap lineMap;
        [SerializeField] private Tilemap washMap;

        private Tilemap map;
        private new TilemapRenderer renderer;
        
#if UNITY_EDITOR
        
        [ContextMenu("Generate")]
        private void Generate()
        {
            if (!TryGetComponent(out map))
                return;
            
            if (!TryGetComponent(out renderer))
                return;

            Clear();

            string[] guids = AssetDatabase.FindAssets("t: TileReference");
            List<TileReference> references = new List<TileReference>();
            foreach (string guid in guids)
            {
                TileReference reference = AssetDatabase.LoadAssetAtPath<TileReference>(AssetDatabase.GUIDToAssetPath(guid));
                if (reference)
                    references.Add(reference);
            }

            lineMap = CreateClone("Line Map");
            lineMap.gameObject.layer = GetLayerIndex(Settings.LineLayer);
            SceneVisibilityManager.instance.Hide(lineMap.gameObject, true);
            
            washMap = CreateClone("Wash Map");
            washMap.gameObject.layer = GetLayerIndex(Settings.WashLayer);
            SceneVisibilityManager.instance.Hide(washMap.gameObject, true);
            
            ReplaceTiles(lineMap, TileType.Line, references);
            ReplaceTiles(washMap, TileType.Colour, references);
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            if (lineMap)
            {
                DestroyImmediate(lineMap.gameObject);
                lineMap = null;
            }

            if (washMap)
            {
                DestroyImmediate(washMap.gameObject);
                washMap = null;
            }
        }

        private static int GetLayerIndex(LayerMask mask)
        {
            int index = 0;
            int value = mask.value;

            while (index < 32)
            {
                if ((value & 1) != 0)
                    return index;

                value >>= 1;
                index++;
            }

            return 0;
        }

        private void ReplaceTiles(Tilemap tilemap, TileType type, IEnumerable<TileReference> references)
        {
            foreach (TileReference reference in references)
                tilemap.SwapTile(reference.GetTile(TileType.Preview), reference.GetTile(type));
        }
        
        private Tilemap CreateClone(string mapName)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform.parent;
            go.transform.localPosition = Vector3.zero;
            go.name = mapName;

            Tilemap tilemap = go.AddComponent<Tilemap>();
            CopyValues(map, tilemap);
            
            TilemapRenderer tilemapRenderer = go.AddComponent<TilemapRenderer>();
            CopyValues(renderer, tilemapRenderer);

            return tilemap;
        }
        
        private static void CopyValues<T>(T original, T other) where T : Component
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(original);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(other);
        }
        
#endif
    }
}