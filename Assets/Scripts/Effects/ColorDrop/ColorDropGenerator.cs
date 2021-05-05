using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ColorDropGenerator : MonoBehaviour
{
    // Inspector Accessible Fields
    [Header("Sprite Texture Samples")]
    [SerializeField] private Sprite[] sampleSpriteSources;

    [Header("Renderers")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private RenderTexture dstRenderTexture;

    [Header("Texture Attributes")]
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float minAspectRatio;

    // Fields
    private Material meshMaterial;
    private System.Random random;
    private Texture2D dstTexture;


    void Awake()
    {
        random = new System.Random();

        CombineSpriteTextures();
        RenderTexture();
    }

    private void CombineSpriteTextures()
    {
        dstTexture = new Texture2D(dstRenderTexture.width, dstRenderTexture.height);
        for (int i = 0; i < sampleSpriteSources.Length; i++)
        {
            BlitDrop(sampleSpriteSources[i], dstTexture);
        }
    }

    private void BlitDrop(Sprite src, Texture2D dst)
    {
        // Drop selection
        float scaleX = minScale + (float) (random.NextDouble() * (maxScale - minScale));
        float aspectRatio = minAspectRatio + (float) (random.NextDouble() * (maxScale - minScale));
        float scaleY = scaleX * aspectRatio;

        // Calculate destination
        Rect dstRect = CreateTextureRect(src.textureRect.width, src.textureRect.height, dst.width, dst.height);

        MergeSourceToDestination(src, dst, dstRect, scaleX, scaleY);
    }

    private void MergeSourceToDestination(Sprite src, Texture2D dst, Rect dstRect, float scaleX, float scaleY)
    {
        Color srcColor, dstColor;

        for (int y = 0; y < dstRect.height; y++)
        {
            for (int x = 0; x < dstRect.width; x++)
            {
                // GetPixelBilinear does not give the desired result
                srcColor = src.texture.GetPixel((int)(src.textureRect.x + x / scaleX), (int)(src.textureRect.y + y / scaleY));
                dstColor = dst.GetPixel((int)dstRect.x + x, (int)dstRect.y + y);
                dstColor.a = srcColor.a;

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

    private void RenderTexture()
    {
        meshMaterial = meshRenderer.material;
        meshMaterial.SetTexture("_BaseMap", dstRenderTexture);
        Graphics.Blit(dstTexture, dstRenderTexture);
    }
}
