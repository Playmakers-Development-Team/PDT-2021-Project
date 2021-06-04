using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorDrop.Particle;

namespace ColorDrop
{
    public interface IColorDropParticleRenderer
    {

    }

    public class ColorDropParticleSystem : MonoBehaviour, IColorDropParticleRenderer
    {
        private IColorDropTextureGenerator texDropGenerator;
        public Texture testTex;
        [HideInInspector] public RenderTexture previewTexture;

        [HideInInspector] public ColorDropParticleAttributes particleAttributes;

        [HideInInspector] public Camera targetCamera;

        [HideInInspector] public bool spawnLocation;
        [HideInInspector] public bool canSpawnRandom;
        [HideInInspector] public Transform[] spawnLocations;

        [HideInInspector] public float startDelay;
        [HideInInspector] public float startRotation;

        [HideInInspector] public Color defaultColor;
        [HideInInspector] public float initialAlpha;
        [HideInInspector] public Vector2 dropScale;

        [HideInInspector] public Material material;
        [HideInInspector] public int sortLayer;

        private void Awake()
        {
            texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Preview Texture

        public Texture GeneratePreviewTexture(Texture previewTex)
        {
            if (texDropGenerator == null)
            {
                texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
            }

            Texture2D newRender = texDropGenerator.GenerateDropOnTex(previewTex);
            Texture preview = previewTex;

            Graphics.Blit(newRender, previewTexture);
            return previewTexture;
        }

        public Texture GeneratePreviewRenderTexture()
        {
            Graphics.Blit(testTex, previewTexture);
            Graphics.Blit(previewTexture, material);
            return previewTexture;
        }

        #endregion
    }

}