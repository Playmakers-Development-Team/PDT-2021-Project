using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    public class TileCreator : EditorWindow
    {
        private string tileName;
        private string tileSet;
        
        private Texture2D lineTexture;
        private Texture2D colourTexture;
        
        
        [MenuItem("Window/Background/Tile Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow<TileCreator>();
            window.titleContent =
                new GUIContent("Tile Creator", EditorGUIUtility.IconContent("Grid.PaintTool").image);
            window.Show();
        }

        private void OnGUI()
        {
            tileName = EditorGUILayout.TextField("Tile Name", tileName);
            tileSet = EditorGUILayout.TextField("Tile Set", tileSet);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Line Texture");
            lineTexture =
                (Texture2D) EditorGUILayout.ObjectField(lineTexture, typeof(Texture2D), false);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Colour Texture");
            colourTexture =
                (Texture2D) EditorGUILayout.ObjectField(colourTexture, typeof(Texture2D), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            // 'Create' button
            // TODO: Perhaps derive tileName (and tileSet) from the file names.
            GUI.enabled = true;

            if (GUILayout.Button("Create Tile"))
            {
                Tile tile = CreateInstance<Tile>();
                tile.Create(lineTexture, colourTexture, tileName);
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Instructions for how to use this should go here...", EditorStyles.helpBox);
        }
    }
}
