using System;
using UnityEngine;

namespace Background.Pipeline
{
    /// <summary>
    /// Initialises the background rendering process, deriving the initial line and wash textures from its viewport.
    /// </summary>
    [RequireComponent(typeof(Camera)), AddComponentMenu("Background/Background Camera")]
    public class BackgroundCamera : MonoBehaviour
    {
        [SerializeField] private bool renderOnAwake;
        [Space]
        
        [Header("Texture Parameters")]
        
        [SerializeField] private Vector2Int resolution;
        [SerializeField] private Color canvasColour;
        
        [Header("Output Parameters")]
        
        [SerializeField] private Renderer washRenderer;
        [SerializeField] private Renderer lineRenderer;

        [Header("Pipeline Parameters")]
        
        [SerializeField] private bool overrideGlobalPipeline;
        [SerializeField, HideInInspector] private Pipeline pipeline;
        
        private new Camera camera;
        private RenderTexture washTexture;
        private RenderTexture lineTexture;

        private void Awake()
        {
            // Right now, there is the option to render when the camera is loaded (or on camera Awake),
            // though we usually want to render as the level is loaded which is before the scene is actually loaded.
            if (renderOnAwake)
                Render();
        }

        /// <summary>
        /// We want to call this before the actual scene is loaded, most probably during the loading
        /// of a level. It takes a few milliseconds before the rendering is complete.
        /// </summary>
        public void Render()
        {
            Initialize();
            
            RenderLine();
            RenderWash();

            if (overrideGlobalPipeline)
                BackgroundManager.Execute(pipeline, lineTexture, washTexture);
            else
                BackgroundManager.Execute(lineTexture, washTexture);

            Apply();
        }

        public void Clear()
        {
            Initialize();
            Apply();
        }

        private void Initialize()
        {
            TryGetComponent(out camera);

            RenderTextureDescriptor descriptor =
                new RenderTextureDescriptor(resolution.x, resolution.y, RenderTextureFormat.Default)
                {
                    enableRandomWrite = true
                };

            TryRelease(washTexture);
            TryRelease(lineTexture);

            lineTexture = new RenderTexture(descriptor)
            {
                name = "Line Texture",
                filterMode = FilterMode.Bilinear
            };
            
            washTexture = new RenderTexture(descriptor)
            {
                name = "Wash Texture",
                filterMode = FilterMode.Bilinear
            };
        }

        private void Apply()
        {
            if (Application.isPlaying)
            {
                washRenderer.material.SetTexture(Settings.WashTexturePropertyName, washTexture);
                lineRenderer.material.SetTexture(Settings.LineTexturePropertyName, lineTexture);
            }
#if UNITY_EDITOR
            else
            {
                washRenderer.sharedMaterial.SetTexture(Settings.WashTexturePropertyName, washTexture);
                lineRenderer.sharedMaterial.SetTexture(Settings.LineTexturePropertyName, lineTexture);
            }
#endif
        }

        private void RenderCamera()
        {
            Vector3 scale = Vector3.one * (2 * camera.orthographicSize);
            scale.x *= camera.aspect;
            lineRenderer.transform.localScale = scale;
            washRenderer.transform.localScale = scale;
            
            camera.Render();
        }

        private void RenderLine()
        {
            camera.targetTexture = lineTexture;
            camera.cullingMask = Settings.LineLayer;
            camera.backgroundColor = Color.clear;

            RenderCamera();
        }

        private void RenderWash()
        {
            camera.targetTexture = washTexture;
            camera.cullingMask = Settings.WashLayer;
            camera.backgroundColor = canvasColour;
            
            RenderCamera();
        }
        
        private static void TryRelease(RenderTexture texture)
        {
            if (texture)
                texture.Release();
        }
    }
}
