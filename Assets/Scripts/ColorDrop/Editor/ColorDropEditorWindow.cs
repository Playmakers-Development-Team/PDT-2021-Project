using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ColorDrop.Editor
{
    public class ColorDropEditorWindow : EditorWindow
    {
        // Fields
        private Texture2D gameObject;
        private UnityEditor.Editor gameObjectEditor;
        private Vector2 scrollPosition = Vector2.zero;
        public ColorDropSettings colorSettings;
        private Vector2 dimensions = Vector2.one;

        private ColorDropEditorWindow dropWindow;
        private SerializedObject serializedObject;
        private SerializedProperty currentProperty;

        // Foldout variabels
        private bool showColorSelections;
        private bool showSDFSelections;
        private bool showShapeSelections;
        private bool showTextureSelections;

        GUILayoutOption[] foldoutLayout =
        {
            GUILayout.Width(300),
            GUILayout.MaxWidth(340),
            GUILayout.MinWidth(290),
            GUILayout.Height(40)
        };


        public static void Open(ColorDropSettings settingsObject)
        {
            ColorDropEditorWindow window = GetWindow<ColorDropEditorWindow>("ColorDrop Settings Window");
            Debug.Log(settingsObject);
            window.serializedObject = new SerializedObject(settingsObject);
            window.colorSettings = settingsObject;
            window.dropWindow = window;
        }

        private void OnGUI()
        {
            serializedObject.Update();

            // Update window dimensions
            dimensions = CalculatePropertyWindowDimensions();
            dropWindow.minSize = new Vector2(350, 100);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUIStyle.none, GUIStyle.none, GUILayout.Width(dimensions.x), GUILayout.Height(dimensions.y));

            // Texture Attributes
            DisplayTextureAttributes();

            // Color Selection
            DrawGUILine(Color.grey, 2, 10);
            currentProperty = serializedObject.FindProperty("colorSelections");
            DrawColorSelectionSection();

            // SDF Selection
            //DrawGUILine(Color.grey, 2, 10);
            currentProperty = serializedObject.FindProperty("sdfSelections");
            DrawSDFSelectionSection();

            //gameObject = (Texture2D)EditorGUILayout.ObjectField(gameObject, typeof(Texture2D), true);
            gameObject = new Texture2D(256, 256);

            currentProperty = serializedObject.FindProperty("textureShapes");
            DisplayTextureShapeSection();

            currentProperty = serializedObject.FindProperty("textureSelections");
            DrawTextureSelectionSection();

            //GUILayout.EndArea();
            GUILayout.EndScrollView();

            /*GUIStyle bgColor = new GUIStyle();
            bgColor.normal.background = Texture2D.grayTexture;

            if (gameObject != null)
            {
                if (gameObjectEditor == null)
                    gameObjectEditor = UnityEditor.Editor.CreateEditor(gameObject);

                gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);
            }

            serializedObject.ApplyModifiedProperties();*/
        }

        private Vector2 CalculatePropertyWindowDimensions()
        {
            Vector2 dimensions = Vector2.zero;
            dimensions.x = dropWindow.position.width;
            dimensions.y = dropWindow.position.height;
            return dimensions;
        }

        private void DrawArrayProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;

            foreach (SerializedProperty p in prop)
            {
                GUILayout.BeginVertical("box", foldoutLayout);
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
                    EditorGUILayout.PropertyField(p, drawChildren, GUILayout.MaxWidth(dimensions.x), GUILayout.MinWidth(dimensions.x/3));
                }
                GUILayout.EndVertical();
            }
        }
        
        private void DrawSimpleArrayProperties(SerializedProperty prop, bool drawChildren)
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
                    EditorGUI.indentLevel++;
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren, GUILayout.MaxWidth(dimensions.x), GUILayout.MinWidth(dimensions.x / 3));
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DisplayTextureAttributes()
        {
            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Texture Attributes", EditorStyles.boldLabel);

            currentProperty = serializedObject.FindProperty("scaleWidth");
            EditorGUILayout.PropertyField(currentProperty, false);
            currentProperty = serializedObject.FindProperty("scaleHeight");
            EditorGUILayout.PropertyField(currentProperty, false);

            GUILayout.EndVertical();
        }

        private void DrawColorSelectionSection()
        {
            string lastPropPath = string.Empty;

            GUIStyle foldoutStyle = new GUIStyle("RL Background");
            foldoutStyle.margin = new RectOffset(10, 10, 5, 5);
            foldoutStyle.padding = new RectOffset(20, 20, 5, 5);
            foldoutStyle.normal.background = Texture2D.grayTexture;


            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical("GroupBox", foldoutLayout);
            GUILayout.Label("Color Selection", EditorStyles.boldLabel);
            GUILayout.Space(5);


            showColorSelections = EditorGUILayout.Foldout(showColorSelections, "Colors");
            if (showColorSelections)
            {
                GUILayout.BeginVertical("GroupBox", foldoutLayout);
                DrawArrayProperties(currentProperty, true);
                GUILayout.EndVertical();
                /*
                GUILayoutOption[] buttonLayout =
                {
                    GUILayout.MaxWidth(50),
                    GUILayout.MinWidth(dimensions.x/2 - 10),
                    GUILayout.Height(40)
                };
                EditorGUI.indentLevel = 0;

                GUIStyle actionButtons = new GUIStyle();
                actionButtons.margin = new RectOffset(5, 5, 0, 0);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal(GUILayout.Width(200));

                if (GUILayout.Button("Add New Color", buttonLayout))
                {
                    Debug.Log("button Pressed");
                    colorSettings.CreateNewColorSelection();
                }

                if (GUILayout.Button("Delete Last Color", buttonLayout))
                {
                    Debug.Log("button Pressed");
                    //colorSettings.CreateNewColorSelection();
                }

                GUILayout.EndHorizontal();*/
            }

            GUILayout.EndVertical();
        }

        private void DrawSDFSelectionSection()
        {
            string lastPropPath = string.Empty;
            Vector2 dimensions = CalculatePropertyWindowDimensions();

            // GUIStyle bgColor = new GUIStyle();
            // bgColor.normal.background = ColorToTexture(new Color(0.3f, 0.3f, 0.3f));

            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("SDF Attribute Selection", EditorStyles.boldLabel);
            GUILayout.Space(5);

            showSDFSelections = EditorGUILayout.Foldout(showSDFSelections, "SDF");
            if (showSDFSelections)
            {
                DrawArrayProperties(currentProperty, true);

                GUILayoutOption[] buttonLayout =
                {
                    GUILayout.Width(600),
                    GUILayout.Height(50)
                };

                GUILayout.Space(20);
                if (GUILayout.Button("Add New SDF Paramaters", buttonLayout))
                {
                    Debug.Log("button Pressed");
                    colorSettings.CreateNewSDFSelection();
                }
            }

            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        private void DisplayTextureShapeSection()
        {
            string lastPropPath = string.Empty;

            // GUIStyle bgColor = new GUIStyle();
            // bgColor.normal.background = ColorToTexture(new Color(0.3f, 0.3f, 0.3f));

            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Texture Shapes List", EditorStyles.boldLabel);
            GUILayout.Space(5);

            showShapeSelections = EditorGUILayout.Foldout(showShapeSelections, "Shapes");
            if (showShapeSelections)
            {
                DrawSimpleArrayProperties(currentProperty, true);

                GUILayoutOption[] buttonLayout =
                {
                    GUILayout.MaxWidth(dimensions.x),
                    GUILayout.MinWidth(dimensions.x/2),
                    GUILayout.Height(50)
                };

                GUILayout.Space(20);
                if (GUILayout.Button("Add New Texture", buttonLayout))
                {
                    Debug.Log("button Pressed");
                    colorSettings.CreateNewTextureShape();
                }
            }

            GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        private void DrawTextureSelectionSection()
        {
            string lastPropPath = string.Empty;
            Vector2 dimensions = CalculatePropertyWindowDimensions();

            // GUIStyle bgColor = new GUIStyle();
            // bgColor.normal.background = ColorToTexture(new Color(0.3f, 0.3f, 0.3f));

            EditorGUI.indentLevel = 1;
            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Texture List", EditorStyles.boldLabel);
            GUILayout.Space(5);

            showTextureSelections = EditorGUILayout.Foldout(showTextureSelections, "Texutres");
            if (showTextureSelections)
            {
                DrawSimpleArrayProperties(currentProperty, true);

                GUILayoutOption[] buttonLayout =
                {
                    GUILayout.Width(600),
                    GUILayout.Height(50)
                };

                GUILayout.Space(20);
                if (GUILayout.Button("Add New Texture", buttonLayout))
                {
                    Debug.Log("button Pressed");
                    colorSettings.CreateNewTextureSelection();
                }
            }

                GUILayout.EndVertical();
            EditorGUI.indentLevel = 0;
        }

        private void RemoveButton()
        {
            GUILayoutOption[] subButton =
            {
                GUILayout.Width(50),
                GUILayout.Height(20)
            };

            GUILayout.Space(20);
            if (GUILayout.Button("Delete Color", subButton))
            {
                colorSettings.CreateNewColorSelection();
            }
        }

        private void OutlinedBox(Color borderColor, Color bgColor, int outlineWeight, int width, int height, int repetitions)
        {
            Rect innerRect = new Rect(dropWindow.position.x, dropWindow.position.y, width, height * repetitions);

            EditorGUI.DrawRect(innerRect, bgColor);
        }

        private Texture2D ColorToTexture(Color color)
        {
            Texture2D tex = Texture2D.whiteTexture;

            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, color);
                }
            }

            tex.Apply();
            return tex;
        }

        private void DrawGUILine(Color color, int thickness, int padding)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x = (dimensions.x / 2) / 2;
            r.width = dimensions.x / 2;
            EditorGUI.DrawRect(r, color);
        }
    }
}
