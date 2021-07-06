using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
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
        [HideInInspector] public float minStartRotation;
        [HideInInspector] public float maxStartRotation;
        [HideInInspector] public Color defaultColor;
        [HideInInspector] public float initialAlpha;
        [HideInInspector] public Vector2 dropScale;
        [HideInInspector] public bool useSceneColor;

        // Emitter Properties
        [HideInInspector] public int rateOverTime;
        [HideInInspector] public int maxParticleCountInView;
        [HideInInspector] public bool canSpawnRandom;
        [HideInInspector] public Transform[] spawnLocations;

        // Renderer
        [HideInInspector] public Camera targetCamera;
        [HideInInspector] public RenderTexture templateTextureRenderTexture;
        [HideInInspector] public Material material;
        [HideInInspector] public bool canRun;

        // Previewer
        [HideInInspector] public RenderTexture previewTexture;

        private GameObject[] particleObjects;
        private IColorDropParticle[] particleInstances;
        private IColorDropTextureGenerator texDropGenerator;
        private IColorSampler colorSampler;

        private ColorDropSettings settings;
        private SimpleTimer particleTimer;
        private Vector2 viewPlaneDimension = Vector2.one;

        private int particleViewCount = 0;
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
            colorSampler.InitialiseColorSampler();

            particleTimer = new SimpleTimer(rateOverTime, Time.deltaTime);
            particleObjects = new GameObject[maxParticleCountInView];
            particleInstances = new IColorDropParticle[maxParticleCountInView];
        }

        private void Update()
        {
            if (!canRun) return;

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
            Vector3 spawnPosition = canSpawnRandom ? NewLocationWithinViewBounds() : SpawnInSpawnLocation();
            float spawnRotation = UnityEngine.Random.Range(minStartRotation, maxStartRotation);

            ColorDropParticleAttributes attributes = new ColorDropParticleAttributes
            {
                particleScale = UnityEngine.Random.Range(dropScale.x, dropScale.y),
                renderTexture = newTex,
                dropMaterial = material,
                defaultColor = this.defaultColor,
                primaryColorSample = colorSampler.SampleColorFromScreenSpace(Camera.main.WorldToScreenPoint(spawnPosition), 6),
                textureDetail = settings.SelectRandomisedTextureDetail(),
                texturePattern = settings.SelectRandomisedTexturePattern()
            };

            IColorDropParticle particle = Instantiate(settings.particlePrefab, spawnPosition, Quaternion.Euler(0, 0, spawnRotation)).GetComponent<IColorDropParticle>();
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
            float xDimension = viewPlaneDimension.x * 0.8f / 2;
            float yDimension = viewPlaneDimension.y * 0.8f / 2;
            newPosition.x = (UnityEngine.Random.Range(1, 10) >= 5 ? 1 : -1) * (UnityEngine.Random.Range(0, xDimension)) + targetCamera.transform.position.x;
            newPosition.y = (UnityEngine.Random.Range(1, 10) >= 5 ? 1 : -1) * (UnityEngine.Random.Range(0, yDimension)) + targetCamera.transform.position.y;
            return newPosition;
        }

        private Vector3 SpawnInSpawnLocation()
        {
            // Gets random location within index
            return spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)].position;
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
            Transform[] objects = FindObjectsOfType<Transform>();
            List<Transform> collectedPositions = objects.Where(x => x.CompareTag("DecalSpawnPosition")).ToList();
            spawnLocations = collectedPositions.ToArray();

            if (spawnLocations.Length == 0)
            {
                Debug.LogWarning("There are no transform positions with tag: 'DecalSpawnPosition'");
            }
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(viewPlaneDimension.x, 0, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(-viewPlaneDimension.x, 0, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(0, viewPlaneDimension.y, 0) + targetCamera.transform.position, Vector3.one);
            Gizmos.DrawCube(new Vector3(0, -viewPlaneDimension.y, 0) + targetCamera.transform.position, Vector3.one);
        }
    }
}