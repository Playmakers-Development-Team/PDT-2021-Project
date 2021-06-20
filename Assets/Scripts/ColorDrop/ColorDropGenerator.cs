using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ColorDrop
{
    public interface IColorDropTextureGenerator
    {
        void InitialiseTextureGenerator(ColorDropSettings settings);
        RenderTexture GenerateDropTexture(RenderTexture templateBase);
        Texture2D GeneratePreviewTex(RenderTexture templateBase);
    }

    public class ColorDropGenerator : MonoBehaviour, IColorDropTextureGenerator
    {
        private RenderTexture dstRenderTexture;
        public ColorDropSettings colorDropSettings;

        [Header("Texture Attributes")]

        private float minScale = 1;
        private float maxScale = 1;
        private float minAspectRatio = 1;

        // Fields
        private Material meshMaterial;
        private System.Random random;
        private Texture2D dstTexture;

        public Sprite testTex;

        private int texWidth;
        private int texHeight;

        public void InitialiseTextureGenerator(ColorDropSettings settings)
        {
            this.colorDropSettings = settings;
        }

        public RenderTexture GenerateDropTexture(RenderTexture templateBase)
        {
            random = new System.Random();
            texWidth = templateBase.width;
            texHeight = templateBase.height;

            dstRenderTexture = new RenderTexture(templateBase);
            dstRenderTexture.Release();
            CreateNewTexture(templateBase.width, templateBase.height);
            CombineSpriteTextures();
            Graphics.Blit(dstTexture, dstRenderTexture);
            return dstRenderTexture;
        }

        public Texture2D GeneratePreviewTex(RenderTexture templateBase)
        {
            random = new System.Random();
            texWidth = templateBase.width;
            texHeight = templateBase.height;

            CreateNewTexture(templateBase.width, templateBase.height);
            CombineSpriteTextures();
            return dstTexture;
        }

        private void CreateNewTexture(int width, int height)
        {
            dstTexture = new Texture2D(width, height);
            Color pixelSample;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelSample = dstTexture.GetPixel(x, y);
                    pixelSample.a = 0;
                    dstTexture.SetPixel(x, y, pixelSample);
                }
            }

            dstTexture.Apply();
        }

        private void CombineSpriteTextures()
        {
            for (int i = 0; i < colorDropSettings.textureShapes.Length; i++)
            {
                BlitDrop(colorDropSettings.textureShapes[i], dstTexture);
            }
            //BlitDrop(testTex, dstTexture);
        }

        private void BlitDrop(Sprite src, Texture2D dst)
        {
            // Drop selection
            float scaleX = minScale + (float)(random.NextDouble() * (maxScale - minScale));
            float aspectRatio = minAspectRatio + (float)(random.NextDouble() * (maxScale - minScale));
            float scaleY = scaleX * aspectRatio;

            // Calculate destination
            Rect dstRect = CreateTextureRect(texWidth, texHeight, dst.width, dst.height);

            MergeSourceToDestination(src, dst, dstRect);
        }

        private void MergeSourceToDestination(Sprite src, Texture2D dst, Rect dstRect)
        {
            Color srcColor, dstColor;
            float scaleMultiplier = src.textureRect.width / dst.width;

            for (int y = 0; y < dstRect.height; y++)
            {
                for (int x = 0; x < dstRect.width; x++)
                {
                    // GetPixelBilinear does not give the desired result
                    srcColor = src.texture.GetPixel((int)(src.textureRect.x + x * scaleMultiplier), (int)(src.textureRect.y + y * scaleMultiplier));
                    dstColor = dst.GetPixel(x, y);
                    dstColor.a = srcColor.a + dstColor.a;

                    // Write the new pixel value to tex using the max between src and dst alpha values
                    dstColor = new Color(0, 0, 0, Math.Max(srcColor.a, dstColor.a));

                    dstTexture.SetPixel((int)dstRect.x + x, (int)dstRect.y + y, dstColor);
                }
            }

            dstTexture.Apply();
        }

        private Rect CreateTextureRect(float srcWidth, float srcHeight, int dstWidth, int dstHeight)
        {
            Rect dstRect = new Rect()
            {
                width = srcWidth,
                height = srcHeight
            };

            dstRect.x = random.Next(0, dstWidth - dstWidth);
            dstRect.y = random.Next(0, dstWidth - dstHeight);

            return dstRect;
        }

        /*private void RenderTexture()
        {
            Graphics.Blit(dstTexture, dstRenderTexture);
            meshMaterial = meshRenderer.sharedMaterial;
            meshMaterial.SetTexture("_BaseMap", dstRenderTexture);
        }*/

    }

    [Serializable]
    public struct ColorSelection
    {
        public Color primaryColor;
        public Color borderColor;
        [Range(0, 1)]
        public float alpha;
    }

    [Serializable]
    public struct SDFSelection
    {
        public float textureSmoothing;

        [Header("Border")]
        public float borderSmoothing;
        public float borderSize;
    }

}