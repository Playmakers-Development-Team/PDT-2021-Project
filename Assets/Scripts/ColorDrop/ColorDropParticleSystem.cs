using System.Collections;
using System.Collections.Generic;
using System;
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
        private IColorDropMeshGenerator meshGenerator;

        //public Texture testTex;
        //public Texture2D result;
        public MeshRenderer testMesh;
        [HideInInspector] public RenderTexture previewTexture;

        [HideInInspector] public ColorDropParticleAttributes particleAttributes;

        [HideInInspector] public Camera targetCamera;
        [HideInInspector] public RenderTexture templateTextureRenderTexture;

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
            meshGenerator = this.GetComponent<IColorDropMeshGenerator>();
        }


        #region Preview Texture

        public Texture GeneratePreviewTexture()
        {
            if (texDropGenerator == null)
            {
                texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
            }

            return texDropGenerator.GeneratePreviewTex(templateTextureRenderTexture);
        }

        public void GenerateToTestMesh()
        {
            if (texDropGenerator == null)
            {
                texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
            }

            RenderTexture newRender = texDropGenerator.GenerateDropTexture(templateTextureRenderTexture);
            testMesh.sharedMaterial.SetTexture("_BaseMap", newRender);
        }

        #endregion

        #region Emitter Functions

        public void CollectSpawnLocations()
        {
            
        }

        #endregion
    }

}