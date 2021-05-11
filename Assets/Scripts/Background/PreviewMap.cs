using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
    [RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
    public class PreviewMap : MonoBehaviour
    {
        [SerializeField] private Tilemap lineMap;
        [SerializeField] private Tilemap fillMap;
        [SerializeField] private Tilemap washMap;

        private Tilemap map;
        private new TilemapRenderer renderer;

        private readonly List<TileReference> references = new List<TileReference>();

#if UNITY_EDITOR

        private void GenerateLine()
        {
            lineMap = CreateClone("Line Map");
            lineMap.gameObject.layer = GetLayerIndex(Settings.LineLayer);
            ReplaceTiles(lineMap, TileType.Line);
        }

        private void GenerateFill()
        {
            fillMap = CreateClone("Fill Map");
            ReplaceTiles(fillMap, TileType.Colour);
        }

        private void GenerateWash()
        {
            washMap = CreateClone("Wash Map");
            washMap.gameObject.layer = GetLayerIndex(Settings.WashLayer);
            ReplaceTiles(washMap, TileType.Colour);

            BoundsInt bounds = washMap.cellBounds;

            for (int x = bounds.position.x; x < bounds.position.x + bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y < bounds.position.y + bounds.size.y; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase inWash = washMap.GetTile(position);
                    TileBase inFill = fillMap.GetTile(position);

                    if (!inWash || inFill)
                        continue;
                    
                    Tile replacement = references.Find(reference => reference.HasTile(inWash as Tile))?.GetTile(TileType.Fill);
                    if (replacement)
                        washMap.SetTile(position, replacement);
                }
            }
        }

        private void Initialise()
        {
            TryGetComponent(out map);
            TryGetComponent(out renderer);
            
            references.Clear();
            string[] guids = AssetDatabase.FindAssets("t: TileReference");
            foreach (string guid in guids)
            {
                TileReference reference = AssetDatabase.LoadAssetAtPath<TileReference>(AssetDatabase.GUIDToAssetPath(guid));
                if (reference)
                    references.Add(reference);
            }
        }
        
        public void GenerateStageOne()
        {
            Initialise();

            Clear();

            GenerateLine();
            GenerateFill();
            
            SceneVisibilityManager.instance.Hide(lineMap.gameObject, false);
            SceneVisibilityManager.instance.Hide(map.gameObject, false);
            
            EditorGUIUtility.PingObject(fillMap.gameObject);
            Selection.activeObject = fillMap.gameObject;
        }

        public void GenerateStageTwo()
        {
            Initialise();
            
            Clear(ref washMap);
            
            GenerateWash();

            Clear(ref fillMap);
            
            SceneVisibilityManager.instance.Hide(washMap.gameObject, false);
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            Clear(ref lineMap);
            Clear(ref fillMap);
            Clear(ref washMap);

            if (SceneVisibilityManager.instance)
                SceneVisibilityManager.instance.Show(map.gameObject, false);
        }

        public bool CanProgress() => lineMap && fillMap;
        
        private static void Clear(ref Tilemap tilemap)
        {
            if (!tilemap)
                return;

            DestroyImmediate(tilemap.gameObject);

            tilemap = null;
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

        private void ReplaceTiles(Tilemap tilemap, TileType type)
        {
            foreach (TileReference reference in references)
                tilemap.SwapTile(reference.GetTile(TileType.Preview), reference.GetTile(type));
        }
        
        private Tilemap CreateClone(string mapName)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
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