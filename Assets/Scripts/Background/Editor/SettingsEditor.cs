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
            pipelineProperty = serializedObject.FindProperty("globalPipeline");
            
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

            UpdatePipeline();
        }

        // BUG: Multiple issues with null properties, disposed serialized objects, unable to enter text, etc.
        public override void OnInspectorGUI()
        {
            // Null checks
            if (pipelineProperty.objectReferenceValue is null)
                pipelineEditor = null;
            else if (pipelineEditor is null) 
                UpdatePipeline();
            
            // Default inspector
            DrawDefaultInspector();
            
            // Pipeline field
            EditorGUI.BeginChangeCheck();
            pipelineProperty.objectReferenceValue = (Pipeline) EditorGUILayout.ObjectField("Global Pipeline",
                pipelineProperty.objectReferenceValue, typeof(Pipeline), false);
            if (EditorGUI.EndChangeCheck())
                UpdatePipeline();
            
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            // Global pipeline header
            CoreEditorUtils.DrawSplitter();
            Rect headerRect = GUILayoutUtility.GetRect(1f, 20f);
            headerRect.width += 4;
            headerRect.xMin = 0;
            EditorGUI.DrawRect(headerRect, new Color(0.1647058f, 0.1647058f, 0.1647058f, 1));
            string headerText = pipelineProperty.objectReferenceValue is null
                ? "No Pipeline Assigned!"
                : $"{pipelineProperty.objectReferenceValue.name} Settings (Global)";
            EditorGUI.LabelField(headerRect, headerText, pipelineHeaderStyle);

            // Global pipeline editor
            pipelineScroll = EditorGUILayout.BeginScrollView(pipelineScroll);
            
            if (pipelineEditor is null)
                EditorGUILayout.HelpBox("No global Pipeline assigned.", MessageType.Error);
            else
                pipelineEditor.OnInspectorGUI();
            
            EditorGUILayout.EndScrollView();
        }

        private void UpdatePipeline()
        {
            if (pipelineEditor)
                DestroyImmediate(pipelineEditor);

            if (pipelineProperty?.objectReferenceValue is null) 
                return;
            
            pipelineEditor = CreateEditor(pipelineProperty.objectReferenceValue);
        }
    }
}
