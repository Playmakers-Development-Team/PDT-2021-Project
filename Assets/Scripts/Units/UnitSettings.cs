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
        public static GameObject GetPrefab<T>() where T : UnitDataBase
        {
            return Instance.unitPrefabs.Find(o => !(o.GetComponent<UnitBase<T>>() is null));
        }
}
}
