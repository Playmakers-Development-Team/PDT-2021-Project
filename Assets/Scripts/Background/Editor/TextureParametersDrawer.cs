using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    [CustomPropertyDrawer(typeof(TextureParameters))]
    public class TextureParametersDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect texturePosition = position;
            texturePosition.height = EditorGUIUtility.singleLineHeight;
            
            SerializedProperty texture = property.FindPropertyRelative("texture");
            EditorGUI.PropertyField(texturePosition, texture, new GUIContent(property.displayName));

            int indent = EditorGUI.indentLevel++;
            
            Rect tilingPosition = new Rect(texturePosition.position, position.size);
            tilingPosition.y += texturePosition.height + 2f;
            tilingPosition.height = EditorGUIUtility.singleLineHeight;
            
            SerializedProperty tiling = property.FindPropertyRelative("tiling");
            
            EditorGUILayout.BeginHorizontal();
            
            Rect controlRect = EditorGUI.PrefixLabel(tilingPosition, new GUIContent("Tiling"));
            EditorGUI.PropertyField(controlRect, tiling, new GUIContent());
            
            EditorGUILayout.EndHorizontal();

            Rect offsetPosition = new Rect(tilingPosition.position, position.size);
            offsetPosition.y += tilingPosition.height + 2f;
            offsetPosition.height = EditorGUIUtility.singleLineHeight;
            
            SerializedProperty offset = property.FindPropertyRelative("offset");
            
            EditorGUILayout.BeginHorizontal();
            
            controlRect = EditorGUI.PrefixLabel(offsetPosition, new GUIContent("Offset"));
            EditorGUI.PropertyField(controlRect, offset, new GUIContent());
            
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => base.GetPropertyHeight(property, label) * 3f + 2f;
    }
}
