using System;
using Background.Masking;
using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(MaskController))]
    public class MaskControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.enabled = false;

            //EditorGUILayout.LabelField($"Final scale: {speed.floatValue * time.floatValue} ");
            
            GUI.enabled = true;
            
            MaskController controller = target as MaskController;
            if (!controller)
                return;

            if (GUILayout.Button("Instantiate Mask"))
            {
                GameObject mask = controller.CreateMask();
                EditorGUIUtility.PingObject(mask);
                Selection.activeObject = mask;
            }

            GUI.enabled = Application.isPlaying;
            
            if (GUILayout.Button("Spread"))
                controller.Spread();
            
            if (GUILayout.Button("Retract"))
                controller.Retract();

            GUI.enabled = true;
        }
    }
}
