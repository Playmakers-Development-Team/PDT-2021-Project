using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty pipelineProperty;
        private UnityEditor.Editor pipelineEditor;

        private Vector2 pipelineScroll = Vector2.zero;

        private static GUIStyle pipelineHeaderStyle;

        private void OnEnable()
        {
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

            pipelineProperty = serializedObject.FindProperty("globalPipeline");
            Pipeline pipeline = pipelineProperty.objectReferenceValue as Pipeline;

            if (pipeline is null)
                return;

            pipelineEditor = CreateEditor(pipeline);
        }

        public override void OnInspectorGUI()
        {
            // Null checks
            if (pipelineProperty.objectReferenceValue is null)
                pipelineEditor = null;
            else if (pipelineEditor is null)
                OnEnable();
            
            // Default inspector
            DrawDefaultInspector();
            EditorGUILayout.Space();

            // Global pipeline header
            CoreEditorUtils.DrawSplitter();
            Rect headerRect = GUILayoutUtility.GetRect(1f, 20f);
            headerRect.width += 4;
            headerRect.xMin = 0;
            EditorGUI.DrawRect(headerRect, new Color(0.1647058f, 0.1647058f, 0.1647058f, 1));
            EditorGUI.LabelField(headerRect, "Global Pipeline", pipelineHeaderStyle);

            // Global pipeline editor
            pipelineScroll = EditorGUILayout.BeginScrollView(pipelineScroll);
            
            EditorGUILayout.Space();
            
            if (pipelineEditor is null)
                EditorGUILayout.HelpBox("No global Pipeline assigned.", MessageType.Error);
            else
                pipelineEditor.OnInspectorGUI();
            
            EditorGUILayout.EndScrollView();
        }
    }
}
