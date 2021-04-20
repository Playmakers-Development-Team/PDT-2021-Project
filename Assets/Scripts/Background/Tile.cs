using System.IO;
using UnityEditor;
using UnityEngine;

namespace Background
{
    // https://answers.unity.com/questions/1527237/save-generated-texture2d-as-texture-type-sprite.html
    public class Tile : ScriptableObject
    {
        [SerializeField] private string tileName;
        
        [SerializeField] private Sprite line;
        [SerializeField] private Sprite colour;
        [SerializeField] private Sprite fill;
        
        
        public void Create(Texture2D lineTex, Texture2D colourTex, string tileName)
        {
            // Assign initial parameters
            this.tileName = tileName;
            
            // Generate additional parameters
            RenderTexture fillRT =
                new RenderTexture(colourTex.width, colourTex.height, 0,
                    colourTex.graphicsFormat, colourTex.mipmapCount)
                {
                    enableRandomWrite = true,
                    useMipMap = true
                };
            fillRT.Create();
            Graphics.CopyTexture(colourTex, fillRT);
            
            // TODO: Commented for now, uncomment when background renderer implemented.
            // Settings.TileCompute.SetTexture(0, "original", fillRT);
            // Settings.TileCompute.Dispatch(0, fillRT.width / 8, fillRT.height / 8, 1);

            Texture2D fillTex =
                new Texture2D(fillRT.width, fillRT.height, colourTex.format, false);
            RenderTexture.active = fillRT;
            fillTex.ReadPixels(new Rect(0, 0, fillRT.width, fillRT.height), 0, 0);
            fillTex.Apply();
            
            // Apply tile import settings to all 3 textures...
            
            // Save textures to disk with proper import settings...
            
            // Load the textures from disk as sprites to assign to member variables.

            // Save this to disk
            const string directory = "/ScriptableObjects/Background/Tiles/";
            Directory.CreateDirectory(Application.dataPath + directory);
            AssetDatabase.CreateAsset(this, "Assets" + directory + tileName + ".asset");
        }
    }
}
