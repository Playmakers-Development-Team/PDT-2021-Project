using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Units
{
    public class UnitSettings : ScriptableObject
    {
        // Member variables
        [SerializeField] private List<GameObject> unitPrefabs;
        
        private static UnitSettings instance;
        private const string resourcePath = "Settings/UnitSettings/UnitSettings";

        
        // Properties
        private static UnitSettings Instance
        {
            get
            {
                if (instance || (instance =
                    Resources.Load<UnitSettings>(resourcePath)))
                    return instance;
                
#if UNITY_EDITOR
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources/" +
                                                    resourcePath);
                AssetDatabase.CreateAsset(instance = CreateInstance<UnitSettings>(),
                    "Assets/Resources/" + resourcePath + ".asset");
#endif

                return instance;
            }
        }


        // Functions
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

        public static string[] GetNames<T>() where T : UnitDataBase
        {
            List<string> names = new List<string>();
            
            foreach (GameObject prefab in Instance.unitPrefabs)
            {
                if (prefab.GetComponent<UnitBase<T>>() is null)
                    continue;

                names.Add(prefab.name);
            }

            return names.ToArray();
        }
}
}
