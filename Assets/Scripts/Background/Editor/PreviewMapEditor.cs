using System;
using Background.Pipeline;
using Background.Tiles;
using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(PreviewMap))]
    public class PreviewMapEditor : UnityEditor.Editor
    {
        private SerializedProperty previewCamera;

        private void OnEnable()
        {
            previewCamera = serializedObject.FindProperty("previewCamera");
        }

        public override void OnInspectorGUI()
        {
            if (!(target is PreviewMap map))
                return;

            if (GUILayout.Button("1. Generate Line and Wash Maps"))
                map.Generate();

            bool guiState = GUI.enabled;
            GUI.enabled = map.CanFinalise();
            if (GUILayout.Button("2. Finalise"))
                map.Finalise();
            GUI.enabled = guiState;
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Reset"))
                map.Clear();
            
            EditorGUILayout.Space();

            previewCamera.objectReferenceValue =
                EditorGUILayout.ObjectField("Preview Camera", previewCamera.objectReferenceValue, typeof(BackgroundCamera), true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
