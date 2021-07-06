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
        private new SerializedObject serializedObject;
        private SerializedProperty currentProperty;
        private ColorDropParticleSystem particleSystem;

        private bool isSpawnFoldoutShowing = false;

        /* Objects for texture preview */
        private Texture previewTex;

        // GUI Layout Parameter Fields
        private GUILayoutOption[] globalFoldout;
        private GUILayoutOption[] listLayout;
        private GUILayoutOption[] previewTexLayout;
        private GUILayoutOption[] sectionFoldout;
        private GUILayoutOption[] headerLabelLayout;

        // GUI Styles
        private GUIStyle headerLabelStyle;
        private GUIStyle sectionStyle;
        private GUIStyle globalStyle;

        private void OnEnable()
        {
            headerTex = EditorGUIUtility.Load("Assets/Resources/EditorUI/ColorDrop/inspector-header-tex.png") as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            particleSystem = (ColorDropParticleSystem)target;

            if (!previewTex)
            {
                previewTex = new Texture2D(256, 256);
            }

            if (headerTex == null)
            {
                headerTex = EditorGUIUtility.Load("Assets/Resources/EditorUI/ColorDrop/inspector-header-tex.png") as Texture2D;
            }

            serializedObject = new SerializedObject(particleSystem);
            serializedObject.Update();

            InstantiateLayoutStyles();


            headerLabelStyle.fontSize = 14;
            headerLabelStyle.fontStyle = FontStyle.Bold;
            headerLabelStyle.normal.background = headerTex;
            headerLabelStyle.normal.textColor = Color.black;

            // Inspector Layout Content
            DrawParticleAttributesSectionGUI();
            DrawEmitterSectionGUI();
            DrawRenderSectionGUI();
            DrawTexturePreviewGUI();

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void InstantiateLayoutStyles()
        {
            globalFoldout = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(40)
            };

            listLayout = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25),
            };

            previewTexLayout = new GUILayoutOption[]
            {
                GUILayout.Height(256),
                GUILayout.Width(256),
            };

            sectionFoldout = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(40)
            };

            headerLabelLayout = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
            };

            sectionStyle = new GUIStyle("GroupBox")
            {
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(1, 1, 1, 1)
            };

            globalStyle = new GUIStyle("Box")
            {
                padding = new RectOffset(8, 8, 8, 8),
                margin = new RectOffset(0, 0, 0, 0)
            };

            headerLabelStyle = new GUIStyle("Box")
            {
                padding = new RectOffset(2, 2, 5, 5),
                margin = new RectOffset(1, 1, 1, 1),
                richText = true,
            };
        }

        private void DrawParticleAttributesSectionGUI()
        {
            GUILayout.Space(10);

            GUILayout.BeginVertical(globalStyle, globalFoldout);

            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Particle Attributes", headerLabelStyle, headerLabelLayout);

            GUILayout.BeginHorizontal();
            currentProperty = serializedObject.FindProperty("minStartRotation");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("maxStartRotation");
            CreatePropertyGUI();

            GUILayout.EndHorizontal();

            currentProperty = serializedObject.FindProperty("defaultColor");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("initialAlpha");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("dropScale");
            CreatePropertyGUI();

            GUILayout.EndVertical();
        }

        private void DrawEmitterSectionGUI()
        {
            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Emitter", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("rateOverTime");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("maxParticleCountInView");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("canSpawnRandom");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("spawnLocations");
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(currentProperty, false, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
            EditorGUI.indentLevel = 0;

            if (GUILayout.Button("Collect all Spawn Locations"))
            {
                particleSystem.CollectSpawnLocations();
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        private void DrawRenderSectionGUI()
        {
            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Renderer", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("targetCamera");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("templateTextureRenderTexture");
            CreatePropertyGUI();

            GUILayout.Space(10);

            currentProperty = serializedObject.FindProperty("material");
            CreatePropertyGUI();

            GUILayout.EndVertical();
        }

        private void CreatePropertyGUI()
        {
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(currentProperty, false, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
        }

        private void DrawTexturePreviewGUI()
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Alpha Previewer", headerLabelStyle, headerLabelLayout);
            GUILayout.FlexibleSpace();

            currentProperty = serializedObject.FindProperty("previewTexture");
            CreatePropertyGUI();

            GUILayout.Space(5);

            if (previewTex)
            {
                Rect r = EditorGUILayout.GetControlRect(previewTexLayout);
                r.x = (Screen.width / 2) - (previewTex.width / 2);

                EditorGUI.DrawTextureAlpha(r, previewTex);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Preview Color Drop"))
            {
                Debug.Log("Generating preview color drop");
                previewTex = particleSystem.GeneratePreviewTexture();
            }

            GUILayout.EndVertical();
        }

    }
}
