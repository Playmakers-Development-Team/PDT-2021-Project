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
        public GameObject particlePrefab;
        [HideInInspector] public ColorDropParticleAttributes particleAttributes;

        // Particle Attributes
        [HideInInspector] public float startDelay;
        [HideInInspector] public float startRotation;
        [HideInInspector] public Color defaultColor;
        [HideInInspector] public float initialAlpha;
        [HideInInspector] public Vector2 dropScale;

        // Emitter Properties
        [HideInInspector] public int rateOverTime;
        [HideInInspector] public int maxParticleCountInView;
        [HideInInspector] public bool spawnOnLocation;
        [HideInInspector] public bool canSpawnRandom;
        [HideInInspector] public Transform[] spawnLocations;

        // Renderer
        [HideInInspector] public Camera targetCamera;
        [HideInInspector] public RenderTexture templateTextureRenderTexture;
        [HideInInspector] public Material material;
        [HideInInspector] public int sortLayer;

        // Previewer
        [HideInInspector] public RenderTexture previewTexture;

        private IColorDropTextureGenerator texDropGenerator;
        private ColorDropSettings settings;
        private int particleViewCount = 0;
        private SimpleTimer particleTimer;
        private Vector2 viewPlaneDimension = Vector2.one;

        private void Awake()
        {
            settings = Resources.Load("Settings/Effects/ColorDropSetting") as ColorDropSettings;
            texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
            particleTimer = new SimpleTimer(rateOverTime, Time.deltaTime);
        }

        private void Update()
        {
            if (particleViewCount > maxParticleCountInView) return;

            particleTimer.TickTimer();

            if (particleTimer.CheckTimeIsUp())
            {
                CreateParticle();
                particleTimer.ResetTimer();
            }
        }

        #region Particle Creation

        public void CreateParticle()
        {
            RenderTexture newTex = texDropGenerator.GenerateDropTexture(templateTextureRenderTexture);

            ColorDropParticleAttributes attributes = new ColorDropParticleAttributes
            {
                particleScale = UnityEngine.Random.Range(dropScale.x, dropScale.y),
                renderTexture = newTex,
                dropMaterial = material
            };

            IColorDropParticle particle = Instantiate(settings.particlePrefab, NewLocationWithinViewBounds(), Quaternion.identity).GetComponent<IColorDropParticle>();
            particle.BeginParticle(attributes);
            particleViewCount++;
        }

        public Vector3 NewLocationWithinViewBounds()
        {
            DetermineCameraPlaneBoundaries();
            Vector3 newPosition = Vector3.zero;
            float xDimension = viewPlaneDimension.x / 2;
            float yDimension = viewPlaneDimension.y / 2;
            newPosition.x = (UnityEngine.Random.Range(1, 10) >= 5 ? 1 : -1) * (UnityEngine.Random.Range(0, xDimension) + xDimension) + targetCamera.transform.position.x;
            newPosition.y = (UnityEngine.Random.Range(1, 10) >= 5 ? 1 : -1) * (UnityEngine.Random.Range(0, yDimension) + yDimension) + targetCamera.transform.position.y;
            return newPosition;
        }

        private void DetermineCameraPlaneBoundaries()
        {
            viewPlaneDimension.x = targetCamera.orthographicSize * targetCamera.aspect;
            viewPlaneDimension.y = viewPlaneDimension.x * (float)(Screen.height / Screen.width);
        }

        #endregion


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
            CreateParticle();
        }

        #endregion

        #region Emitter Functions

        public void CollectSpawnLocations()
        {
            
        }

        #endregion
    }

    public class SimpleTimer
    {
        // Fields
        private float intervalLength;
        private float timeLeft;
        private float deltaTime;

        public SimpleTimer(float intervalLength, float deltaTime)
        {
            this.intervalLength = intervalLength;
            this.timeLeft = intervalLength;
            this.deltaTime = deltaTime;
        }

        public void UpdateTimerAttributes(float intervalLength, float deltaTime)
        {
            this.intervalLength = intervalLength;
            this.deltaTime = deltaTime;
        }

        public void TickTimer()
        {
            timeLeft -= deltaTime;
        }

        public bool CheckTimeIsUp()
        {
            return timeLeft <= 0;
        }

        public void ResetTimer()
        {
            timeLeft = intervalLength;
        }
    }

}