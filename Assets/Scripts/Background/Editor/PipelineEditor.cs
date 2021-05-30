using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Background.Pipeline.Features;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Background.Editor
{
    [CustomEditor(typeof(Pipeline.Pipeline))]
    public class PipelineEditor : UnityEditor.Editor
    {
        private SerializedProperty features;
        private readonly List<UnityEditor.Editor> editors = new List<UnityEditor.Editor>();

        private void Awake()
        {
            Initialise();
        }

        private void OnEnable()
        {
            Initialise();
        }

        private void Initialise()
        {
            features = serializedObject.FindProperty("features");
        }

        public override void OnInspectorGUI()
        {
            if (features is null || features.arraySize != editors.Count)
                UpdateEditors();
            
            DrawFeatures();
        }

        private void OnDestroy()
        {
            features = null;
        }

        private void UpdateEditors()
        {
            foreach (UnityEditor.Editor editor in editors)
                DestroyImmediate(editor);
            
            editors.Clear();

            if (features?.serializedObject is null)
                return;

            for (int i = 0; i < features.arraySize; i++)
            {
                editors.Add(CreateEditor(features.GetArrayElementAtIndex(i).objectReferenceValue));
            }
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(target);
        }

        private void DrawFeatures()
        {
            if (features is null || features.arraySize == 0)
                EditorGUILayout.HelpBox("No Features added", MessageType.Info);
            else
            {
                CoreEditorUtils.DrawSplitter();
                for (int i = 0; i < features.arraySize; i++)
                {
                    SerializedProperty featureProperty = features.GetArrayElementAtIndex(i);
                    bool renamed = DrawRendererFeature(i, ref featureProperty);
                    if (renamed)
                        return;
                    CoreEditorUtils.DrawSplitter();
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Feature", EditorStyles.miniButton))
                AddFeatureMenu();
        }

        private bool DrawRendererFeature(int index, ref SerializedProperty featureProperty)
        {
            UnityEngine.Object featureObj = featureProperty.objectReferenceValue;

            if (featureObj is null)
            {
                EditorGUILayout.HelpBox("Feature is null", MessageType.Error);
                return false;
            }

            bool renamed = false;
            bool changed = false;
            string title = ObjectNames.GetInspectorTitle(featureObj);

            UnityEditor.Editor editor = editors[index];
            SerializedObject featureSerializedObject = editor.serializedObject;
            featureSerializedObject.Update();

            EditorGUI.BeginChangeCheck();
            SerializedProperty active = featureSerializedObject.FindProperty("active");
            bool display = CoreEditorUtils.DrawHeaderToggle(title, featureProperty, active,
                pos => OnContextClick(pos, index));
            changed |= EditorGUI.EndChangeCheck();

            if (display)
            {
                EditorGUI.BeginChangeCheck();
                featureObj.name =
                    ValidateName(
                        EditorGUILayout.DelayedTextField("Name", featureObj.name));
                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;

                    AssetDatabase.SaveAssets();
                    ProjectWindowUtil.ShowCreatedAsset(target);

                    renamed = true;
                }
                
                EditorGUI.BeginChangeCheck();
                editor.OnInspectorGUI();
                changed |= EditorGUI.EndChangeCheck();
                
                EditorGUILayout.Space();

            }

            if (changed)
            {
                featureSerializedObject.ApplyModifiedProperties();
                serializedObject.ApplyModifiedProperties();
                ForceSave();
            }

            return renamed;
        }
        
        private void OnContextClick(Vector2 position, int id)
        {
            var menu = new GenericMenu();

            if (id == 0)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveFeature(id, -1));

            if (id == features.arraySize - 1)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveFeature(id, 1));

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveFeature(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private void AddFeatureMenu()
        {
            GenericMenu menu = new GenericMenu();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Feature>();
            foreach (Type type in types)
            {
                string path = type.Name;
                path = Regex.Replace(
                    Regex.Replace(path, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled),
                    "([A-Z])([A-Z][a-z])", "$1 $2", RegexOptions.Compiled);
                
                menu.AddItem(new GUIContent(path), false, AddFeature, type.Name);
            }
            menu.ShowAsContext();
        }
        
        private void AddFeature(object type)
        {
            serializedObject.Update();

            ScriptableObject feature = CreateInstance((string) type);
            feature.name = $"New {(string) type}";

            if (EditorUtility.IsPersistent(target))
                AssetDatabase.AddObjectToAsset(feature, target);

            features.arraySize++;
            SerializedProperty featureProperty =
                features.GetArrayElementAtIndex(features.arraySize - 1);
            featureProperty.objectReferenceValue = feature;

            if (EditorUtility.IsPersistent(target))
                ForceSave();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveFeature(int index)
        {
            SerializedProperty property = features.GetArrayElementAtIndex(index);
            UnityEngine.Object feature = property.objectReferenceValue;
            property.objectReferenceValue = null;

            features.DeleteArrayElementAtIndex(index);
            UpdateEditors();
            serializedObject.ApplyModifiedProperties();
            
            if (feature)
                DestroyImmediate(feature, true);

            ForceSave();
        }

        private void MoveFeature(int index, int offset)
        {
            serializedObject.Update();
            features.MoveArrayElement(index, index + offset);
            UpdateEditors();
            serializedObject.ApplyModifiedProperties();
            
            ForceSave();
        }
        
        private static string ValidateName(string name)
        {
            name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");
            return name;
        }
    }
}
