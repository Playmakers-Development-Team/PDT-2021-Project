using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    public class UnitSettings : ScriptableObject
    {
        [SerializeField] private List<GameObject> unitPrefabs = new List<GameObject>();
        
        private static UnitSettings instance;
        
        private const string resourcePath = "Settings/UnitSettings/";

        
        private static UnitSettings Instance
        {
            get
            {
                if (instance)
                    return instance;

                instance = Resources.Load<UnitSettings>(resourcePath + "UnitSettings");
                
                if (instance)
                    return instance;
                
#if UNITY_EDITOR
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/" +
                                                    resourcePath);
                AssetDatabase.CreateAsset(instance = CreateInstance<UnitSettings>(),
                    "Assets/Resources/" + resourcePath + "UnitSettings.asset");
#endif

                return instance;
            }
        }

        
        public static GameObject GetPrefab(string unitName)
        {
            GameObject prefab = Instance.unitPrefabs.Find(o => o.name.Equals(unitName));
            
            if (prefab is null)
            {
                throw new System.Exception(
                    $"Prefab for Unit with name '{unitName}' not assigned to UnitSettings" +
                    " ScriptableObject at Assets/Resources/Settings/UnitSettings.asset.");
            }
            
            return prefab;
        }

        public static string[] GetNames<T>() where T : UnitData
        {
            List<string> names = new List<string>();
            
            foreach (GameObject prefab in Instance.unitPrefabs)
            {
                if (prefab.GetComponent<Unit<T>>() is null)
                    continue;

                names.Add(prefab.name);
            }

            return names.ToArray();
        }
}
}
