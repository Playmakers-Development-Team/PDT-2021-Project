using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    public class SettingsWindow : EditorWindow
    {
        private UnityEditor.Editor editor;
        
        [MenuItem("Window/Background/Settings Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("Background Settings",
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