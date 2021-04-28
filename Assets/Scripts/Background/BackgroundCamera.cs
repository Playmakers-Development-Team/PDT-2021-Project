using UnityEngine;

namespace Background
{
    [RequireComponent(typeof(Camera)), AddComponentMenu("Background/Background Camera")]
    public class BackgroundCamera : MonoBehaviour
    {
        [Header("Texture Settings")]
        
        [SerializeField] private Vector2Int resolution;
        [SerializeField] private Color canvasColour;
        
        [Header("Output Parameters")]
        
        [SerializeField] private Renderer washRenderer;
        [SerializeField] private Renderer lineRenderer;

        [Header("Features")]
        [SerializeField] private LineFeature lineFeature;
        [SerializeField] private DisplacementFeature displacementFeature;
        
        private new Camera camera;
        private RenderTexture washTexture;
        private RenderTexture lineTexture;
        

        [ContextMenu("Render")]
        public void Render()
        {
            Initialize();
            
            RenderLine();
            RenderWash();

            Apply();
        }

        [ContextMenu("Clear")]
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
            
            lineFeature.input = lineTexture;
            lineFeature.Execute();
        }

        private void RenderWash()
        {
            camera.targetTexture = washTexture;
            camera.cullingMask = Settings.WashLayer;
            camera.backgroundColor = canvasColour;
            
            RenderCamera();
            
            // TODO: Dispatch each wash Feature here...

            // TODO: Uncomment once the compute code has been written...
            // displacementFeature.input = washTexture;
            // displacementFeature.Execute();
        }

        
        #region Utility

        private static void TryRelease(RenderTexture texture)
        {
            if (texture)
                texture.Release();
        }
        
        #endregion
    }
}
