using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Background
{
    [RequireComponent(typeof(Tilemap), typeof(TilemapRenderer))]
    public class PreviewMap : MonoBehaviour
    {
        [SerializeField] private Tilemap lineMap;
        [SerializeField] private Tilemap washMap;

        [SerializeField] private BackgroundCamera previewCamera;

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

        private void GenerateWash()
        {
            washMap = CreateClone("Wash Map");
            washMap.gameObject.layer = GetLayerIndex(Settings.WashLayer);
            ReplaceTiles(washMap, TileType.Colour);
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
        
        public void Generate()
        {
            Initialise();

            Clear();
            
            ReplaceTiles(map, TileType.Preview);

            GenerateLine();
            GenerateWash();

            SceneVisibilityManager.instance.Hide(map.gameObject, false); 
            SceneVisibilityManager.instance.Hide(lineMap.gameObject, false);

            EditorGUIUtility.PingObject(washMap.gameObject);
            Selection.activeObject = washMap.gameObject;
        }

        public void Finalise()
        {
            Initialise();
            
            SceneVisibilityManager.instance.Hide(map.gameObject, true);

            gameObject.layer = GetLayerIndex(Settings.PreviewLayer);
            
            if (previewCamera)
                previewCamera.Render();

            Selection.activeObject = null;
        }

        public bool CanFinalise() => lineMap && washMap;

        [ContextMenu("Clear")]
        public void Clear()
        {
            Initialise();
            
            Clear(ref lineMap);
            Clear(ref washMap);

            SceneVisibilityManager.instance.Show(map.gameObject, false);

            gameObject.layer = 0;
            
            if (previewCamera)
                previewCamera.Clear();
        }
        
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
            {
                BoundsInt bounds = tilemap.cellBounds;

                for (int x = bounds.position.x; x < bounds.position.x + bounds.size.x; x++)
                {
                    for (int y = bounds.position.y; y < bounds.position.y + bounds.size.y; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        TileBase current = tilemap.GetTile(position);

                        if (!current || !reference.HasTile(current as Tile))
                            continue;
                        
                        Tile replacement = reference.GetTile(type);
                        if (replacement)
                            tilemap.SetTile(position, replacement);
                    }
                }
            }
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