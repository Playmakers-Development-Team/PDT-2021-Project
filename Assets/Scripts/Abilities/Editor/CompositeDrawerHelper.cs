using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Abilities.Editor
{
    public class CompositeDrawerHelper
    {
        private readonly Type type;
        private readonly SerializedProperty compositeProperty;
        private readonly SerializedProperty affectTypeProperty;
        private readonly SerializedProperty compositeTypeProperty;
        private SerializedProperty childProperty;
        private FieldInfo childField;
        
        private float MiddleSpacing => EditorGUIUtility.standardVerticalSpacing * 3f;
        public bool IsCompositeTypeNone => compositeTypeProperty.enumValueIndex != 0;

        public CompositeDrawerHelper(Type type, SerializedProperty property, 
                                     string affectType, string compositeType)
        {
            this.type = type;
            this.compositeProperty = property;
            this.affectTypeProperty = property.FindPropertyRelative(affectType);
            this.compositeTypeProperty = property.FindPropertyRelative(compositeType);
            UpdateChild();
        }

        public bool IsNameInitialised(SerializedProperty property) =>
            !string.IsNullOrEmpty(property.FindPropertyRelative("name").stringValue);
        
        public void UpdatePropertyDisplayName()
        {
            // If something has changed, apply the changes first before looking up the name using reflection
            compositeProperty.serializedObject.ApplyModifiedProperties();
            SerializedProperty nameProperty = compositeProperty.FindPropertyRelative("name");
            nameProperty.stringValue = GetDisplayName();
            // Forces every drawer above, including the EffectDrawer to update
            GUI.changed = true;
            compositeProperty.serializedObject.ApplyModifiedProperties();
        }
        
        public string GetDisplayName()
        {
            string affectString = ((AffectType) affectTypeProperty.enumValueIndex).ToString();
            string childString = "....";

            if (childField != null && typeof(IDisplayable).IsAssignableFrom(childField.FieldType))
            {
                IDisplayable displayable = GetPropertyObject(childProperty) as IDisplayable;

                if (displayable != null)
                    childString = $" {displayable.DisplayName}";
            }

            return $"{affectString}{childString}";
        }

        public float GetPropertyHeight()
        {
            if (!compositeProperty.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            float baseHeight = EditorGUIUtility.singleLineHeight * 2f
                               + MiddleSpacing
                               + EditorGUIUtility.standardVerticalSpacing;

            float costHeight = childProperty != null
                ? GetChildProperties().Sum(p =>
                    EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing)
                : 0f;

            return baseHeight + EditorGUIUtility.singleLineHeight + costHeight;
        }

        public void OnPropertyGUI(Rect position)
        {
            Rect namePosition = new Rect(position.x, position.y, position.width,
                EditorGUIUtility.singleLineHeight);
            compositeProperty.isExpanded = EditorGUI.Foldout(namePosition, compositeProperty.isExpanded,
                new GUIContent(compositeProperty.displayName));

            if (compositeProperty.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                Rect affectTypePosition = new Rect(position.x, namePosition.y + namePosition.height,
                    position.width, EditorGUIUtility.singleLineHeight);
                Rect costTypePosition = new Rect(position.x,
                    affectTypePosition.y + affectTypePosition.height
                                         + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(affectTypePosition, affectTypeProperty);
                EditorGUI.PropertyField(costTypePosition, compositeTypeProperty);

                float costY = costTypePosition.y + costTypePosition.height;
                Rect childPosition = new Rect(position.x, costY, position.width,
                    position.height - costY);

                if (IsCompositeTypeNone)
                    OnChildGUI(childPosition);
                
                EditorGUI.indentLevel--;
            }
            
            UpdateChild();
        }

        private void OnChildGUI(Rect position)
        {
            if (childProperty == null)
                return;

            Rect currentRect = new Rect(position.x, position.y, position.width, MiddleSpacing);

            foreach (SerializedProperty currentProperty in GetChildProperties())
            {
                currentRect = new Rect(position.x,
                    currentRect.y + currentRect.height + EditorGUIUtility.standardVerticalSpacing,
                    position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(currentRect, currentProperty);
            }
        }

        private void UpdateChild()
        {
            this.childField = GetChildField();
            this.childProperty = childField != null
                ? compositeProperty.FindPropertyRelative(childField.Name)
                : null;
        }

        private FieldInfo GetChildField()
        {
            foreach (var field in type.GetFields(BindingFlags.Public 
                                                 | BindingFlags.NonPublic 
                                                 | BindingFlags.Instance 
                                                 | BindingFlags.FlattenHierarchy))
            {
                if (field.IsDefined(typeof(CompositeChildAttribute)))
                {
                    CompositeChildAttribute attribute = (CompositeChildAttribute) field
                        .GetCustomAttribute(typeof(CompositeChildAttribute));

                    if (attribute.EnumValue == compositeTypeProperty.enumValueIndex)
                    {
                        return field;
                    }
                }
            }

            return null;
        }

        private IEnumerable<SerializedProperty> GetChildProperties()
        {
            SerializedProperty currentProperty = childProperty.Copy();
            bool hasNext = currentProperty.NextVisible(true);

            while (hasNext && currentProperty.depth == childProperty.depth + 1)
            {
                yield return currentProperty.Copy();
                hasNext = currentProperty.NextVisible(true);
            }
        }
        
        // This can probably be moved to an Editor utility class
        private static object GetPropertyObject(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            // We replace every array into "#" for further array processing later
            var fieldPath = Regex.Replace(property.propertyPath, @"Array\.data\[(.)\]", "#$1")
                .Split('.');

            foreach (var path in fieldPath)
            {
                if (path.Contains("#"))
                {
                    int i = 0;
                    int index = int.Parse(path.Replace("#", ""));
                    bool isElementFound = false;

                    foreach (var element in (IEnumerable) obj)
                    {
                        if (i++ == index)
                        {
                            obj = element;
                            isElementFound = true;
                            break;
                        }
                    }

                    if (!isElementFound)
                        return null;
                }
                else
                {
                    FieldInfo field = obj.GetType().GetField(path, 
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    
                    if (field == null)
                        return null;
                    
                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }
    }
}
