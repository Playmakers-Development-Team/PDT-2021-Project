using System.IO;
using System.Linq;
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

        private Texture2D fillTexture;

        private static GUIStyle textureLabelStyle;
        private static GUIStyle strongStyle;
        private static string instructions;
        
        private const string parentDirectory = "Assets/ScriptableObjects/Tiles/";
        
        
        [MenuItem("Window/Background/Tile Creator")]
        private static void ShowWindow()
        {
            var window = GetWindow<TileCreator>();
            window.titleContent =
                new GUIContent("Tile Creator", EditorGUIUtility.IconContent("Grid.PaintTool").image);
            window.Show();
        }

        private void OnEnable()
        {
            Color textColour = new Color(0.769f, 0.769f, 0.769f, 1);
            
            strongStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = textColour
                },
                wordWrap = true
            };

            textureLabelStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = textColour
                }
            };

            instructions =
                File.ReadAllText(Application.dataPath + "/Editor/tile-creator-instructions.txt");
        }

        private void OnGUI()
        {
            tileSet = EditorGUILayout.TextField("Tile Set", tileSet);
            tileName = EditorGUILayout.TextField("Tile Name", tileName);

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();

            lineTexture = TextureField("Line Texture", lineTexture);
            colourTexture = TextureField("Colour Texture", colourTexture);
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            // 'Create' button
            bool tileSetValid = !(tileSet is null) && tileSet.Length > 0 && !tileSet.Contains(" ");
            bool tileNameValid = !(tileName is null) && tileName.Length > 0;
            GUI.enabled = tileSetValid && tileNameValid && lineTexture && colourTexture;

            if (GUILayout.Button("Create Tile"))
                CreateTile();

            GUI.enabled = true;

            EditorGUILayout.Space();
            GUILayout.Label("Path: " + parentDirectory + tileSet + "/" + tileName, strongStyle);
            
            EditorGUILayout.Space();
            Divider();
            EditorGUILayout.Space();
            
            GUILayout.Label(
                new GUIContent(instructions, EditorGUIUtility.IconContent("_Help@2x").image),
                EditorStyles.helpBox);
        }

        private void CreateTile()
        {
            // STEP 1. Generate fill texture if it hasn't been assigned.
            RenderTexture fillRT =
                new RenderTexture(colourTexture.width, colourTexture.height, 0)
                {
                    enableRandomWrite = true
                };
            fillRT.Create();

            ComputeShader shader =
                AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Shaders/Tile.compute");
            shader.SetTexture(0, "input", colourTexture);
            shader.SetTexture(0, "output", fillRT);
            shader.Dispatch(0, fillRT.width / 8, fillRT.height / 8, 1);
                
            fillTexture =
                new Texture2D(fillRT.width, fillRT.height, TextureFormat.RGBA32, true);
                
            RenderTexture.active = fillRT;
            fillTexture.ReadPixels(new Rect(0, 0, fillRT.width, fillRT.height), 0, 0);
            fillTexture.Apply();
            
            // STEP 2. Save textures to file.
            string path = AssetDatabase.GetAssetPath(colourTexture);
            path = path.Replace(Path.GetFileName(path), "");
            path += tileName + "_fill.png";
            
            byte[] bytes = fillTexture.EncodeToPNG();
            DestroyImmediate(fillTexture);
            
            string dataPath = Application.dataPath;
            dataPath = dataPath.Remove(dataPath.Length - 6);
            
            File.WriteAllBytes(dataPath + path, bytes);
            AssetDatabase.Refresh();
            
            fillTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            // STEP 3. Apply correct import settings to textures.
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(colourTexture),
                tileName + "_colour");
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(lineTexture),
                tileName + "_line");
            ApplyImportSettings(lineTexture);
            ApplyImportSettings(colourTexture);
            ApplyImportSettings(fillTexture);
            
            AssetDatabase.Refresh();

            // STEP 4. Create tile ScriptableObject
            TileSpriteReference tileSpriteReference = CreateInstance<TileSpriteReference>();
            tileSpriteReference.name = tileName;
            tileSpriteReference.Initialize(
                AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(lineTexture)).
                    OfType<Sprite>().ToArray()[0],
                AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(colourTexture)).
                    OfType<Sprite>().ToArray()[0],
                AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(fillTexture)).
                    OfType<Sprite>().ToArray()[0]
            );

            // STEP 5. Save tile as asset
            string directory = "/ScriptableObjects/Background/Tiles/" + tileSet + "/";
            Directory.CreateDirectory(Application.dataPath + directory);
            AssetDatabase.CreateAsset(tileSpriteReference, "Assets" + directory + tileName + ".asset");
            
            AssetDatabase.Refresh();
            Selection.activeObject = tileSpriteReference;
            EditorGUIUtility.PingObject(tileSpriteReference);
        }

        private static void ApplyImportSettings(Texture texture)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;

            if (!importer)
            {
                Debug.LogError("AssetImporter as TextureImporter was null!");
                return;
            }

            TextureImporterSettings settings = new TextureImporterSettings();
            
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 512;
            importer.spritePivot = 
                new Vector2(0.5f, 0.5f * ((float) texture.width / texture.height));
            
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int) SpriteAlignment.Custom;
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);
            
            importer.SaveAndReimport();
        }

        private static Texture2D TextureField(string label, Texture2D texture)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Label(label, textureLabelStyle);
            Texture2D input =
                (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false);
            
            EditorGUILayout.EndVertical();

            return input;
        }

        private static void Divider()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}
