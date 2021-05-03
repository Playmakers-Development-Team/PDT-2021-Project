using UnityEditor;
using UnityEngine;

namespace Background.Editor
{
    [CustomPropertyDrawer(typeof(FeatureTexture))]
    public class FeatureTextureDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            SerializedProperty name = property.FindPropertyRelative("name");
            name.stringValue = EditorGUI.TextField(position,
                $"{property.displayName} Name", name.stringValue);
        }

        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
