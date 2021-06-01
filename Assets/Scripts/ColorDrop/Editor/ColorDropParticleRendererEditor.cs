using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    [CustomEditor(typeof(ColorDropParticleSystem))]
    public class ColorDropParticleRendererEditor : UnityEditor.Editor
    {
        private Texture2D headerTex;
        private SerializedObject serializedObject;
        private SerializedProperty currentProperty;

        private void OnEnable()
        {
            headerTex = EditorGUIUtility.Load("Assets/Resources/EditorUI/ColorDrop/inspector-header-tex.png") as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (headerTex == null)
            {
                headerTex = EditorGUIUtility.Load("Assets/Resources/EditorUI/ColorDrop/inspector-header-tex.png") as Texture2D;
            }

            ColorDropParticleSystem particleSystem = (ColorDropParticleSystem)target;
            serializedObject = new SerializedObject(particleSystem);

            GUILayoutOption[] globalFoldout =
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(40)
            };

            GUIStyle globalStyle = new GUIStyle("Box")
            {
                padding = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(0, 0, 0, 0)
            };

            GUILayoutOption[] sectionFoldout =
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(40)
            };

            GUIStyle sectionStyle = new GUIStyle("GroupBox")
            {
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(1, 1, 1, 1)
            };


            GUIStyle headerLabelStyle = new GUIStyle("Box")
            {
                padding = new RectOffset(2, 2, 5, 5),
                margin = new RectOffset(1, 1, 1, 1),
                richText = true,
            };

            headerLabelStyle.fontSize = 14;
            headerLabelStyle.fontStyle = FontStyle.Bold;
            headerLabelStyle.normal.background = headerTex;
            headerLabelStyle.normal.textColor = Color.black;

            GUILayoutOption[] headerLabelLayout =
            {
                GUILayout.ExpandWidth(true),
            };

            // Inspector Layout Content

            GUILayout.Space(10);

            GUILayout.BeginVertical(globalStyle, globalFoldout);

            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Particle Attributes", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("startDelay");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("startRotation");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("defaultColor");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("initialAlpha");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("dropScale");
            CreatePropertyGUI();

            GUILayout.EndVertical();

            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Renderer Attributes", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("material");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("sortLayer");
            CreatePropertyGUI();

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
        private void CreatePropertyGUI()
        {
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(currentProperty, false, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
        }
    }
}
