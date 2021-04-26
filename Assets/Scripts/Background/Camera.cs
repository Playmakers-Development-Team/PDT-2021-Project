using System;
using UnityEngine;

namespace Background
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class Camera : MonoBehaviour
    {
        [SerializeField] private Vector2Int resolution;
        
        [Space]
        
        [SerializeField] private LayerMask washLayer;
        [SerializeField] private Renderer washRenderer;
        [SerializeField] private string washTexturePropertyName;
        
        [Space]
        
        [SerializeField] private LayerMask lineLayer;
        [SerializeField] private Renderer lineRenderer;
        [SerializeField] private string lineTexturePropertyName;
        
        private new UnityEngine.Camera camera;
        private RenderTexture washTexture;
        private RenderTexture lineTexture;
        

        private void Awake()
        {
            TryGetComponent(out camera);
        }

        public void Render()
        {
            
        }
    }
}
