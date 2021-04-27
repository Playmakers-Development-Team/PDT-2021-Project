#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace Background.Utility
{
    #if UNITY_EDITOR

    [InitializeOnLoad]
#endif
    public class GraphicsHelper
    {
        static GraphicsHelper()
        {
            OnLoad();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnLoad()
        {
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);
        }
    }
}

