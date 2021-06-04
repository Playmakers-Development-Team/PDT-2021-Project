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

        private bool isSpawnFoldoutShowing = false;

        /* Objects for texture preview */
        private Texture previewTex;

        private void OnEnable()
        {
            headerTex = EditorGUIUtility.Load("Assets/Resources/EditorUI/ColorDrop/inspector-header-tex.png") as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ColorDropParticleSystem particleSystem = (ColorDropParticleSystem)target;

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

            GUILayoutOption[] globalFoldout =
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(40)
            };

            GUILayoutOption[] listLayout =
            {
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25),
            };

            GUILayoutOption[] previewTexLayout =
            {
                GUILayout.Height(256),
                GUILayout.Width(256),
            };


            /* Editor Styles */

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
            GUILayout.Label("Emitter", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("spawnLocation");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("canSpawnRandom");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("spawnLocations");
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(currentProperty, false, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
            EditorGUI.indentLevel = 0;

            if (GUILayout.Button("Collect all Spawn Locations"))
            {
                Debug.Log("Collection Triggered");
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();

            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Renderer", headerLabelStyle, headerLabelLayout);

            currentProperty = serializedObject.FindProperty("targetCamera");
            CreatePropertyGUI();

            GUILayout.Space(10);

            currentProperty = serializedObject.FindProperty("material");
            CreatePropertyGUI();

            currentProperty = serializedObject.FindProperty("sortLayer");
            CreatePropertyGUI();

            if (GUILayout.Button("Play"))
            {
                Debug.Log("Collection Triggered");
            }


            if (GUILayout.Button("Pause"))
            {
                Debug.Log("Collection Triggered");
            }

            if(GUILayout.Button("Reset"))
            {
                Debug.Log("Collection Triggered");
            }


            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginVertical(sectionStyle, sectionFoldout);
            GUILayout.Label("Previewer", headerLabelStyle, headerLabelLayout);
            GUILayout.FlexibleSpace();

            currentProperty = serializedObject.FindProperty("previewTexture");
            CreatePropertyGUI();

            GUILayout.Space(5);

            if (previewTex)
            {
                Rect r = EditorGUILayout.GetControlRect(previewTexLayout);
                r.x = (Screen.width / 2) - (previewTex.width/2);

                EditorGUI.DrawPreviewTexture(r, previewTex);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Preview Color Drop"))
            {
                Debug.Log("Generating preview color drop");
                previewTex = particleSystem.GeneratePreviewRenderTexture();
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();

            DrawTexturePreviewGUI();

            serializedObject.ApplyModifiedProperties();
        }
        private void CreatePropertyGUI()
        {
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(currentProperty, false, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
        }

        private void DrawArrayProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;

            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawArrayProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width / 3));
                }
            }
        }

        private void DrawTexturePreviewGUI()
        {

        }

    }
}
