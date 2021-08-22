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
        private PreviewMap map;

        private void OnEnable()
        {
            map = target as PreviewMap;
            previewCamera = serializedObject.FindProperty("previewCamera");
        }

        public override void OnInspectorGUI()
        {
            if (map == null)
                return;

            if (GUILayout.Button("1. Generate Line and Wash Maps"))
            {
                map.Generate();
                EditorUtility.SetDirty(map);
            }
            
            bool guiState = GUI.enabled;
            GUI.enabled = map.CanFinalise();
            if (GUILayout.Button("2. Finalise"))
            {
                map.Finalise();
                EditorUtility.SetDirty(map);
            }
            GUI.enabled = guiState;
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Reset"))
            {
                map.Clear();
                EditorUtility.SetDirty(map);
            }
            
            EditorGUILayout.Space();
            
            previewCamera.objectReferenceValue =
                EditorGUILayout.ObjectField("Preview Camera", previewCamera.objectReferenceValue, typeof(BackgroundCamera), true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
