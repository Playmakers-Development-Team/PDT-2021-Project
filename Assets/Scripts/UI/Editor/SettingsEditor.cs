using UnityEditor;
using UnityEngine;

namespace UI.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty attackIcon;
        private SerializedProperty defenceIcon;
        
        private SerializedProperty passionIcon;
        private SerializedProperty apathyIcon;
        private SerializedProperty prideIcon;
        private SerializedProperty humilityIcon;
        private SerializedProperty joyIcon;
        private SerializedProperty sorrowIcon;

        private SerializedProperty attackColour;
        private SerializedProperty defenceColour;
        
        private SerializedProperty passionColour;
        private SerializedProperty apathyColour;
        private SerializedProperty prideColour;
        private SerializedProperty humilityColour;
        private SerializedProperty joyColour;
        private SerializedProperty sorrowColour;

        private int toolbarIndex;
        private Vector2 scrollPosition;
        
        private static readonly string[] toolbarLabels =
        {
            "Icons",
            "Colours"
        };

        private void OnEnable()
        {
            attackIcon = serializedObject.FindProperty("attackIcon");
            defenceIcon = serializedObject.FindProperty("defenceIcon");
            
            passionIcon = serializedObject.FindProperty("passionIcon");
            apathyIcon = serializedObject.FindProperty("apathyIcon");
            prideIcon = serializedObject.FindProperty("prideIcon");
            humilityIcon = serializedObject.FindProperty("humilityIcon");
            joyIcon = serializedObject.FindProperty("joyIcon");
            sorrowIcon = serializedObject.FindProperty("sorrowIcon");
            
            attackColour = serializedObject.FindProperty("attackColour");
            defenceColour = serializedObject.FindProperty("defenceColour");
            
            passionColour = serializedObject.FindProperty("passionColour");
            apathyColour = serializedObject.FindProperty("apathyColour");
            prideColour = serializedObject.FindProperty("prideColour");
            humilityColour = serializedObject.FindProperty("humilityColour");
            joyColour = serializedObject.FindProperty("joyColour");
            sorrowColour = serializedObject.FindProperty("sorrowColour");
        }

        public override void OnInspectorGUI()
        {
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarLabels);
            
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            switch (toolbarIndex)
            {
                case 0:
                    DrawIcons();
                    break;
                
                case 1:
                    DrawColours();
                    break;
            }
            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawIcons()
        {
            EditorGUILayout.PropertyField(attackIcon);
            EditorGUILayout.PropertyField(defenceIcon);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(passionIcon);
            EditorGUILayout.PropertyField(apathyIcon);
            EditorGUILayout.PropertyField(prideIcon);
            EditorGUILayout.PropertyField(humilityIcon);
            EditorGUILayout.PropertyField(joyIcon);
            EditorGUILayout.PropertyField(sorrowIcon);
        }

        private void DrawColours()
        {
            EditorGUILayout.PropertyField(attackColour);
            EditorGUILayout.PropertyField(defenceColour);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(passionColour);
            EditorGUILayout.PropertyField(apathyColour);
            EditorGUILayout.PropertyField(prideColour);
            EditorGUILayout.PropertyField(humilityColour);
            EditorGUILayout.PropertyField(joyColour);
            EditorGUILayout.PropertyField(sorrowColour);
        }
    }
}
