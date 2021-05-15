using System.IO;
using UnityEditor;
using UnityEngine;

namespace Background.Pipeline
{
    /// <summary>
    /// ScriptableObject singleton settings class for the background rendering system.
    /// </summary>
    public class Settings : ScriptableObject
    {
        [Tooltip("The compute shader used to render backgrounds.")]
        [SerializeField] private ComputeShader backgroundCompute;
        
        [Space]
        
        [Tooltip("The layer for preview maps.")]
        [SerializeField] private LayerMask previewLayer;
        
        [Space]
        
        [Tooltip("The layer used to render wash textures.")]
        [SerializeField] private LayerMask washLayer;
        [Tooltip("The name of the albedo texture property in wash materials.")]
        [SerializeField] private string washTexturePropertyName;
        
        [Space]
        
        [Tooltip("The layer used to render line textures.")]
        [SerializeField] private LayerMask lineLayer;
        [Tooltip("THe name of the albedo texture property in line materials.")]
        [SerializeField] private string lineTexturePropertyName;
        
        [Space]
        
        [Tooltip("The compute shader used to generate tile variants.")]
        [SerializeField] private ComputeShader tileCompute;
        
        [SerializeField, HideInInspector] private Pipeline globalPipeline;
        
        private const string instanceDirectory = "Background/Settings/";
        private const string instanceFileName = "settingsInstance";

        private static Settings instance;


        public static ComputeShader BackgroundCompute => Instance.backgroundCompute;
        public static LayerMask PreviewLayer => Instance.previewLayer;
        public static LayerMask WashLayer => Instance.washLayer;
        public static string WashTexturePropertyName => Instance.washTexturePropertyName;
        public static LayerMask LineLayer => Instance.lineLayer;
        public static string LineTexturePropertyName => Instance.lineTexturePropertyName;
        public static ComputeShader TileCompute => Instance.tileCompute;
        public static Pipeline GlobalPipeline => Instance.globalPipeline;
        
        
        public static Settings Instance
        {
            get
            {
                if (instance)
                    return instance;

                if (instance = Resources.Load<Settings>(instanceDirectory + instanceFileName))
                    return instance;
                
#if UNITY_EDITOR
                instance = CreateInstance<Settings>();
                Directory.CreateDirectory(Application.dataPath + "/Resources/" + instanceDirectory);
                AssetDatabase.CreateAsset(instance,
                    "Assets/Resources/" + instanceDirectory + instanceFileName + ".asset");
#endif

                return instance;
            }
        }
    }
} 