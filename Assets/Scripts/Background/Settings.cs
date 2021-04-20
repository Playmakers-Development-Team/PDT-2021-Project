using System.IO;
using UnityEditor;
using UnityEngine;

namespace Background
{
    public class Settings : ScriptableObject
    {
        [SerializeField] private ComputeShader backgroundCompute;
        [SerializeField] private ComputeShader tileCompute;
        
        private const string instanceDirectory = "Background/Settings/";
        private const string instanceFileName = "settingsInstance";

        private static Settings instance;


        public static ComputeShader BackgroundCompute => Instance.backgroundCompute;
        public static ComputeShader TileCompute => Instance.tileCompute;
        
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
