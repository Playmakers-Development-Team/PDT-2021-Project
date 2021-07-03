using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    public interface IColorSampler
    {
        void InitialiseColorSampler();
        Color SampleColorFromScreenSpace(Vector2 screenPos, int sampleDensity);
        bool CheckValidForColorSample();
    }

    public class ColorDecalColorSampler : MonoBehaviour, IColorSampler
    {
        // Fields
        private Camera renderCamera;
        private RenderTexture cameraOpaqueTexture;
        private Texture2D textureData;

        public void InitialiseColorSampler()
        {
            cameraOpaqueTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            renderCamera = Camera.main.GetComponentInChildren<Camera>();
            renderCamera.targetTexture = cameraOpaqueTexture;
            textureData = new Texture2D(Screen.width, Screen.height);
        }
        
        public Color SampleColorFromScreenSpace(Vector2 screenPos, int sampleDensity)
        {
            renderCamera.Render();
            RenderTexture.active = cameraOpaqueTexture;
            Color[] colorSamples = CollectColorSamples(sampleDensity, screenPos);
            Color averageColor = AverageColorSamples(colorSamples);

            return colorSamples[0];
        }

        private Color[] CollectColorSamples(int density, Vector2 center)
        {
            Color[] samples = new Color[density];
            Vector2 sampleOffset = Vector2.zero;
            textureData.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            textureData.Apply();

            for (int i = 0; i < density; i++)
            {
                // sampleOffset = new Vector2(Random.Range(-0.5, 1), Random.Range(-1, 1)); DISABLED FOR NOW
                Vector2 position = center + sampleOffset;
                samples[i] = textureData.GetPixel((int)position.x, (int)position.y);
            }

            return samples;
        }

        private Color AverageColorSamples(Color[] sample)
        {
            Color combinedColor = Color.black;
            
            foreach (Color color in sample)
            {
                combinedColor += color;
            }

            combinedColor *= 0.5f;
            return combinedColor;
        }

        public bool CheckValidForColorSample()
        {
            if (cameraOpaqueTexture == null) return false;
            if (renderCamera == null) return false;
            return true;
        }
    }
}