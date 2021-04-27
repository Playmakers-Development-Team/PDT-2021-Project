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
        
        [SerializeField] private LayerMask washLayer;
        [SerializeField] private Renderer washRenderer;
        [SerializeField] private string washTexturePropertyName;
        
        [Space]
        
        [SerializeField] private LayerMask lineLayer;
        [SerializeField] private Renderer lineRenderer;
        [SerializeField] private string lineTexturePropertyName;

        [Header("Features")]
        [SerializeField] private DisplacementFeature displacement;
        
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
                washRenderer.material.SetTexture(washTexturePropertyName, washTexture);
                lineRenderer.material.SetTexture(lineTexturePropertyName, lineTexture);
            }
#if UNITY_EDITOR
            else
            {
                washRenderer.sharedMaterial.SetTexture(washTexturePropertyName, washTexture);
                lineRenderer.sharedMaterial.SetTexture(lineTexturePropertyName, lineTexture);
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
            camera.cullingMask = lineLayer;
            camera.backgroundColor = Color.clear;

            RenderCamera();
            
            // TODO: Dispatch the Line feature here...
        }

        private void RenderWash()
        {
            camera.targetTexture = washTexture;
            camera.cullingMask = washLayer;
            camera.backgroundColor = canvasColour;
            
            RenderCamera();
            
            // TODO: Dispatch each wash Feature here...

            // TODO: Uncomment once the compute code has been written...
            // displacement.input = washTexture;
            // displacement.Execute();
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
