using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ColorDrop.Particle;

namespace ColorDrop
{
    public interface IColorDropParticleRenderer
    {
        void OnParticleRemoval();
    }

    public class ColorDropParticleSystem : MonoBehaviour, IColorDropParticleRenderer
    {
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

        private GameObject[] particleObjects;
        private IColorDropParticle[] particleInstances;
        private IColorDropTextureGenerator texDropGenerator;
        private IColorSampler colorSampler;
        private ColorDropSettings settings;
        private int particleViewCount = 0;
        private SimpleTimer particleTimer;
        private Vector2 viewPlaneDimension = Vector2.one;

        private float rightBound;
        private float leftBound;
        private float upBound;
        private float downBound;

        private bool isVertOut = false;
        private bool isHorOut = false;

        private void Awake()
        {
            settings = Resources.Load("Settings/Effects/ColorDropSetting") as ColorDropSettings;
            texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
            texDropGenerator.InitialiseTextureGenerator(settings);
            colorSampler = this.GetComponent<IColorSampler>();

            particleTimer = new SimpleTimer(rateOverTime, Time.deltaTime);
            particleObjects = new GameObject[maxParticleCountInView];
            particleInstances = new IColorDropParticle[maxParticleCountInView];
        }

        private void Update()
        {
            IterateThroughParticles();
            RunParticleSpawn();
        }

        #region Particle Creation

        public void RunParticleSpawn()
        {
            if (particleViewCount >= maxParticleCountInView) return;

            particleTimer.TickTimer();

            if (particleTimer.CheckTimeIsUp())
            {
                CreateParticle();
                particleTimer.ResetTimer();
            }
        }

        public void CreateParticle()
        {
            RenderTexture newTex = texDropGenerator.GenerateDropTexture(templateTextureRenderTexture);

            ColorDropParticleAttributes attributes = new ColorDropParticleAttributes
            {
                particleScale = UnityEngine.Random.Range(dropScale.x, dropScale.y),
                renderTexture = newTex,
                dropMaterial = material,
                textureDetail = settings.SelectRandomisedTextureDetail(),
                texturePattern = settings.SelectRandomisedTexturePattern()
            };

            IColorDropParticle particle = Instantiate(settings.particlePrefab, NewLocationWithinViewBounds(), Quaternion.identity).GetComponent<IColorDropParticle>();
            particle.BeginParticle(attributes, this);
            particleViewCount++;

            AddToArray(particle);
        }

        private void AddToArray(IColorDropParticle particle)
        {
            for (int i = 0; i < particleInstances.Length; i++)
            {
                if (particleInstances[i] == null)
                {
                    particleInstances[i] = particle;
                    particleObjects[i] = particle.GetParticleGameObject();
                    return;
                }
            }
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

        private void IterateThroughParticles()
        {
            for (int i = 0; i < particleObjects.Length; i++)
            {
                if (particleObjects[i] != null)
                {
                    if (CheckParticlePositionIsVisible(particleInstances[i].GetParticlePosition()))
                    {
                        particleInstances[i].EndParticle();
                        Destroy(particleObjects[i]);
                        particleInstances[i] = null;
                        particleObjects[i] = null;
                    }
                }
            }
        }

        private bool CheckParticlePositionIsVisible(Vector3 pos)
        {
            rightBound = (viewPlaneDimension.x) + targetCamera.transform.position.x;
            leftBound = targetCamera.transform.position.x - (viewPlaneDimension.x);
            upBound = (viewPlaneDimension.y) + targetCamera.transform.position.y;
            downBound = targetCamera.transform.position.y - (viewPlaneDimension.y);

            isVertOut = pos.y < downBound || pos.y > upBound;
            isHorOut = pos.x < leftBound || pos.x > rightBound;

            return isVertOut && isHorOut;
        }

        public void OnParticleRemoval()
        {
            particleViewCount--;
            particleViewCount = particleViewCount <= 0 ? 0 : particleViewCount;
        }

        private void DetermineCameraPlaneBoundaries()
        {
            viewPlaneDimension.x = targetCamera.orthographicSize * targetCamera.aspect;
            viewPlaneDimension.y = viewPlaneDimension.x * (float)Screen.height / (float)Screen.width;
        }

        #endregion


        #region Preview Texture

        public Texture GeneratePreviewTexture()
        {
            if (texDropGenerator == null || settings == null)
            {
                settings = Resources.Load("Settings/Effects/ColorDropSetting") as ColorDropSettings;
                texDropGenerator = this.GetComponent<IColorDropTextureGenerator>();
                texDropGenerator.InitialiseTextureGenerator(settings);
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
            GameObject[] objects = FindObjectsOfType<GameObject>();
            //spawnLocations = objects.Where(x => x.GetComponent<ISpawn)
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(new Vector3(viewPlaneDimension.x, 0, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(-viewPlaneDimension.x, 0, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(0, viewPlaneDimension.y, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(0, -viewPlaneDimension.y, 0) + targetCamera.transform.position, Vector3.one);
        }
    }
}