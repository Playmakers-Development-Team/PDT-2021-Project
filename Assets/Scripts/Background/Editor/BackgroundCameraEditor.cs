using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(BackgroundCamera))]
    public class BackgroundCameraEditor : UnityEditor.Editor
    {
        private SerializedProperty overrideProperty;
        private SerializedProperty pipelineProperty;
        private UnityEditor.Editor pipelineEditor;

        private static GUIStyle pipelineHeaderStyle;
        
        
        private void OnEnable()
        {
            overrideProperty = serializedObject.FindProperty("overrideGlobalPipeline");
            pipelineProperty = serializedObject.FindProperty("pipeline");
            
            pipelineHeaderStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin
                        ? new Color(0.7686275f, 0.7686275f, 0.7686275f, 1)
                        : Color.black
                }
            };
            
            UpdatePipelineEditor();
        }

        public override void OnInspectorGUI()
        {
            if (!(target is BackgroundCamera camera))
                return;
            
            // Render/clear buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Render"))
                camera.Render();

            if (GUILayout.Button("Clear"))
                camera.Clear();
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Default inspector
            DrawDefaultInspector();

            // Pipeline header
            CoreEditorUtils.DrawSplitter();
            Rect headerRect = GUILayoutUtility.GetRect(1f, 20f);
            headerRect.width += 4;
            headerRect.xMin = 0;
            EditorGUI.DrawRect(headerRect, new Color(0.1647058f, 0.1647058f, 0.1647058f, 1));
            EditorGUI.LabelField(headerRect, "Override Pipeline", pipelineHeaderStyle);
            
            // Pipeline editor
            GUI.enabled = overrideProperty.boolValue;
            if (pipelineEditor)
                pipelineEditor.OnInspectorGUI();
            // pipelineEditor.serializedObject.ApplyModifiedProperties();
            GUI.enabled = true; 
        }

        private void UpdatePipelineEditor()
        {
            if (pipelineEditor)
                DestroyImmediate(pipelineEditor);

            if (pipelineProperty.objectReferenceValue is null)
            {
                pipelineProperty.objectReferenceValue = CreateInstance<Pipeline>();
                serializedObject.ApplyModifiedProperties();
            }

            pipelineEditor = CreateEditor(pipelineProperty.objectReferenceValue);
        }
    }
}
