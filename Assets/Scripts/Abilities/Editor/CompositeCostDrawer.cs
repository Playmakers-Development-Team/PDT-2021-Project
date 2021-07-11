using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Abilities.Costs;
using StatusEffects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Abilities.Editor
{
    [CustomPropertyDrawer(typeof(CompositeCost), true)]
    public class CompositeCostDrawer : PropertyDrawer
    {
        private const string affectTypeName = "affectType";
        private const string costTypeName = "costType";

        private float MiddleSpacing => EditorGUIUtility.standardVerticalSpacing * 3f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            OnPropertyGUI(position, property, label);

            if (EditorGUI.EndChangeCheck() || !IsNameInitialised(property))
            {
                UpdatePropertyDisplayName(property);
            }
        }

        private bool IsNameInitialised(SerializedProperty property) =>
            !string.IsNullOrEmpty(property.FindPropertyRelative("name").stringValue);

        private void UpdatePropertyDisplayName(SerializedProperty property)
        {
            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            nameProperty.stringValue = GetCostDisplayName(property);
            // Forces every drawer above, including the EffectDrawer to update
            GUI.changed = true;
            property.serializedObject.ApplyModifiedProperties();
        }

        private string GetCostDisplayName(SerializedProperty property)
        {
            SerializedProperty affectTypeProperty = property.FindPropertyRelative(affectTypeName);
            SerializedProperty costTypeProperty = property.FindPropertyRelative(costTypeName);

            SerializedProperty costProperty = GetChildCostProperty(property, costTypeProperty);

            string affectString = ((AffectType) affectTypeProperty.enumValueIndex).ToString();
            CostType costType = (CostType) costTypeProperty.enumValueIndex;

            string suffix = costType switch
            {
                CostType.None => "....",
                CostType.Tenet => GetTenetCostDisplayName(costProperty),
                CostType.Shape => GetShapeCostDisplayName(costProperty),
                _ => throw new ArgumentOutOfRangeException(
                    $"{nameof(CostType)} not supported by {nameof(CompositeCostDrawer)}")
            };

            return $"{affectString} {suffix}";
        }

        private string GetShapeCostDisplayName(SerializedProperty property)
        {
            // property should be null if the type does not have shape property
            if (property == null)
                return "can't have shape!";
            
            SerializedProperty shapeProperty = property.FindPropertyRelative("shape");
            SerializedProperty shapeFilterProperty = property.FindPropertyRelative("shapeFilter");
            SerializedProperty countProperty = property.FindPropertyRelative("minCount");
            SerializedProperty costProperty = property.FindPropertyRelative("cost");
            SerializedProperty costTypeProperty = costProperty.FindPropertyRelative("costType");

            CostType costType = (CostType) costTypeProperty.enumValueIndex;

            string shapeName = shapeProperty.objectReferenceValue != null
                ? shapeProperty.objectReferenceValue.name
                : "No Defined Shape";
            string shapeFilterString = ((ShapeFilter) shapeFilterProperty.enumValueIndex)
                .ToString();
            
            // Put spaces between uppercase letters
            shapeFilterString = Regex.Replace(shapeFilterString, @"([A-Z])", " $0")
                .Substring(1);
            
            string costName = costProperty.FindPropertyRelative("name").stringValue;
            string costString = costType != CostType.None ? $" where {costName}" : string.Empty;

            return $"finds {countProperty.intValue} {shapeFilterString} in {shapeName}{costString}";
        }

        private string GetTenetCostDisplayName(SerializedProperty property)
        {
            if (property == null)
                return "can't have tenet cost!";

            SerializedProperty countProperty = property.FindPropertyRelative("count");
            SerializedProperty tenetCostTypeProperty = property.FindPropertyRelative("tenetCostType");
            SerializedProperty tenetTypeProperty = property.FindPropertyRelative("tenetType");

            string tenetCostTypeString =
                ((TenetCostType) tenetCostTypeProperty.enumValueIndex).ToString();
            string tenetString = ((TenetType) tenetTypeProperty.enumValueIndex).ToString();

            return $"{tenetCostTypeString} {countProperty.intValue} {tenetString}";
        }

        private void OnPropertyGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty affectTypeProperty = property.FindPropertyRelative(affectTypeName);
            SerializedProperty costTypeProperty = property.FindPropertyRelative(costTypeName);

            Rect namePosition = new Rect(position.x, position.y, position.width,
                EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(namePosition, property.isExpanded,
                new GUIContent(property.displayName));

            if (property.isExpanded)
            {
                Rect affectTypePosition = new Rect(position.x, namePosition.y + namePosition.height,
                    position.width, EditorGUIUtility.singleLineHeight);
                Rect costTypePosition = new Rect(position.x,
                    affectTypePosition.y + affectTypePosition.height
                                         + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(affectTypePosition, affectTypeProperty);
                EditorGUI.PropertyField(costTypePosition, costTypeProperty);

                float costY = costTypePosition.y + costTypePosition.height;
                Rect costPosition = new Rect(position.x, costY, position.width, position.height - costY);
                
                if ((CostType) costTypeProperty.enumValueIndex != CostType.None)
                    OnChildCostGUI(costPosition, property, costTypeProperty);
            }
        }

        private void OnChildCostGUI(Rect position, SerializedProperty property,
                                    SerializedProperty costTypeProperty)
        {
            SerializedProperty costProperty = GetChildCostProperty(property, costTypeProperty);

            if (costProperty == null)
                return;
            
            Rect currentRect = new Rect(position.x, position.y, position.width, MiddleSpacing);

            foreach (SerializedProperty currentProperty in GetChildCostProperties(costProperty))
            {
                currentRect = new Rect(position.x,
                    currentRect.y + currentRect.height + EditorGUIUtility.standardVerticalSpacing,
                    position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(currentRect, currentProperty);
            }
        }

        private IEnumerable<SerializedProperty> GetChildCostProperties(SerializedProperty costProperty)
        {
            SerializedProperty currentProperty = costProperty.Copy();
            bool hasNext = currentProperty.NextVisible(true);

            while (hasNext && currentProperty.depth == costProperty.depth + 1)
            {
                yield return currentProperty.Copy();
                hasNext = currentProperty.NextVisible(true);
            }
        }

        /// <summary>
        /// Might return null if the property does not support the cost type
        /// </summary>
        private SerializedProperty GetChildCostProperty(SerializedProperty property,
                                                        SerializedProperty costTypeProperty) =>
            (CostType) costTypeProperty.enumValueIndex switch
            {
                CostType.None => null,
                CostType.Tenet => property.FindPropertyRelative("tenetCost"),
                CostType.Shape => property.FindPropertyRelative("shapeCost"),
                _ => throw new ArgumentOutOfRangeException(
                    $"{nameof(CostType)} not supported by {nameof(CompositeCostDrawer)}")
            };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            SerializedProperty costTypeProperty = property.FindPropertyRelative(costTypeName);
            SerializedProperty costProperty = GetChildCostProperty(property, costTypeProperty);

            float baseHeight = EditorGUIUtility.singleLineHeight * 2f
                               + MiddleSpacing
                               + EditorGUIUtility.standardVerticalSpacing;

            float costHeight = costProperty != null
                ? GetChildCostProperties(costProperty)
                    .Sum(p => EditorGUI.GetPropertyHeight(p) + EditorGUIUtility.standardVerticalSpacing)
                : 0f;

            return baseHeight + EditorGUIUtility.singleLineHeight + costHeight;
        }
    }
}
