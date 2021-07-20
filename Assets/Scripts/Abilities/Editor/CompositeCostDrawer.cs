using System;
using Abilities.Costs;
using UnityEditor;
using UnityEngine;

namespace Abilities.Editor
{
    // TODO: Duplicate code, see CompositeBonusDrawer
    [CustomPropertyDrawer(typeof(CompositeCost), true)]
    public class CompositeCostDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            CompositeDrawerHelper compositeDrawerHelper = CreateHelper(property);
            
            compositeDrawerHelper.OnPropertyGUI(position);

            if (EditorGUI.EndChangeCheck() || !CompositeDrawerHelper.IsNameInitialised(property))
                compositeDrawerHelper.UpdatePropertyDisplayName();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            CreateHelper(property).GetPropertyHeight();

        private CompositeDrawerHelper CreateHelper(SerializedProperty property)
        {
            Type type = fieldInfo.FieldType.IsArray
                ? fieldInfo.FieldType.GetElementType()
                : fieldInfo.FieldType;

            return new CompositeDrawerHelper(type, property, "affectType", "costType");
        }
    }
}
