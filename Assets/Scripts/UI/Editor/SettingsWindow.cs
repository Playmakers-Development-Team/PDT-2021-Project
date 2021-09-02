using UnityEditor;
using UnityEngine;

namespace UI.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private UnityEditor.Editor editor;
        
        [MenuItem("Window/UI/Settings")]
        private static void ShowWindow()
        {
            SettingsWindow window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("UI Settings",
                EditorGUIUtility.IconContent("SettingsIcon").image);
            window.Show();
        }

        private void OnEnable()
        {
            editor = UnityEditor.Editor.CreateEditor(Settings.Instance);
        }

        private void OnGUI()
        {
            editor.OnInspectorGUI();
        }
    }
}