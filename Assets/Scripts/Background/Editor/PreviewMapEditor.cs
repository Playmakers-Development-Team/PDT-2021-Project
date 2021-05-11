using System;
using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(PreviewMap))]
    public class PreviewMapEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (!(target is PreviewMap map))
                return;

            if (GUILayout.Button("1. Generate Line and Fill Maps"))
                map.GenerateStageOne();

            bool guiState = GUI.enabled;
            GUI.enabled = map.CanProgress();
            if (GUILayout.Button("2. Generate Wash Map"))
                map.GenerateStageTwo();
            GUI.enabled = guiState;
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Clear Maps"))
                map.Clear();
        }
    }
}
