using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace ColorDrop.Editor
{
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            ColorDropSettings obj = EditorUtility.InstanceIDToObject(instanceID) as ColorDropSettings;

            if (obj != null)
            {
                ColorDropEditorWindow.Open(obj);
                return true;
            }
            return false;
        }
    }

    [CustomEditor(typeof(ColorDropSettings))]
    public class DropSettingsCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                ColorDropEditorWindow.Open((ColorDropSettings)target);
            }
        }
    }
}
