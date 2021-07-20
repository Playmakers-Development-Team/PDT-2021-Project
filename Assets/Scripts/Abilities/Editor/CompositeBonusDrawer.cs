using System;
using Abilities.Bonuses;
using UnityEditor;
using UnityEngine;

namespace Abilities.Editor
{
    // TODO: Duplicate code, see CompositeCostDrawer
    [CustomPropertyDrawer(typeof(CompositeBonus), true)]
    public class CompositeBonusDrawer : PropertyDrawer
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

            return new CompositeDrawerHelper(type, property, "affectType", "bonusType");
        }
    }
}
