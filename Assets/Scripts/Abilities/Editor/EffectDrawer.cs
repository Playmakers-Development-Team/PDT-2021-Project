using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Costs;
using StatusEffects;
using UnityEditor;
using UnityEngine;

namespace Abilities.Editor
{
    [CustomPropertyDrawer(typeof(Effect))]
    public class EffectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            
            if (EditorGUI.EndChangeCheck())
            {
                SerializedProperty damageProperty = property.FindPropertyRelative("damageValue");
                SerializedProperty attackProperty = property.FindPropertyRelative("attackValue");
                SerializedProperty defenceProperty = property.FindPropertyRelative("defenceValue");
                SerializedProperty provideProperty = property.FindPropertyRelative("providingTenet");
                SerializedProperty provideTenetTypeProperty = provideProperty.FindPropertyRelative("tenetType");
                SerializedProperty provideCountProperty = provideProperty.FindPropertyRelative("stackCount");
                
                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                SerializedProperty bonusProperty = property.FindPropertyRelative("bonus");
                SerializedProperty tenetBonusesProperty = bonusProperty.FindPropertyRelative("tenetBonuses");
                SerializedProperty keywordsProperty = property.FindPropertyRelative("keywords");
                SerializedProperty costProperty = property.FindPropertyRelative("cost");
                SerializedProperty tenetCostsProperty =
                    costProperty.FindPropertyRelative("tenetCosts");

                List<string> valueNameList = new List<string>();
                nameProperty.stringValue = string.Empty;
                
                bool hasValues = damageProperty.intValue != 0 
                                    || defenceProperty.intValue != 0
                                    || attackProperty.intValue != 0 
                                    || provideCountProperty.intValue != 0;
                
                if (tenetCostsProperty.arraySize == 0 && keywordsProperty.arraySize == 0)
                    nameProperty.stringValue += "Default, ";

                    // Damage and defence
                if (hasValues)
                {
                    if (damageProperty.intValue != 0)
                        valueNameList.Add($"{damageProperty.intValue} Damage");
                    
                    if (attackProperty.intValue != 0)
                        valueNameList.Add($"{attackProperty.intValue} Attack");
                    
                    if (defenceProperty.intValue != 0)
                        valueNameList.Add($"{defenceProperty.intValue} Defence");
                    
                    if (provideCountProperty.intValue != 0)
                        valueNameList.Add($"Provide {provideCountProperty.intValue} {(TenetType)provideTenetTypeProperty.enumValueIndex}");
                }
                
                if (!hasValues)
                    valueNameList.Add("Nothing");

                nameProperty.stringValue += string.Join(" and ", valueNameList);

                
                if (tenetBonusesProperty.arraySize != 0)
                    nameProperty.stringValue += " " + ProcessTenetBonusesDisplayName(tenetBonusesProperty);
                
                if (tenetCostsProperty.arraySize != 0)
                    nameProperty.stringValue += " " + ProcessTenetCostsDisplayName(tenetCostsProperty);

                if (keywordsProperty.arraySize != 0)
                    nameProperty.stringValue += " " + ProcessKeywordsDisplayName(keywordsProperty);

                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private string ProcessKeywordsDisplayName(SerializedProperty keywordsProperty)
        {
            if (keywordsProperty.arraySize > 0)
            {
                List<string> keywordNameList = new List<string>();

                for (int i = 0; i < keywordsProperty.arraySize; i++)
                {
                    SerializedProperty keywordProperty = keywordsProperty.GetArrayElementAtIndex(i);
                    keywordNameList.Add(keywordProperty.objectReferenceValue.name);
                }

                return $"WITH {string.Join(" and ", keywordNameList)}";
            }
            
            return string.Empty;
        }

        private string ProcessTenetBonusesDisplayName(SerializedProperty tenetBonusesProperty)
        {
            List<string> tenetBonusStringList = new List<string>();

            for (int i = 0; i < tenetBonusesProperty.arraySize; i++)
            {
                SerializedProperty tenetBonusProperty = tenetBonusesProperty.GetArrayElementAtIndex(i);
                
                SerializedProperty affectTypeProperty = tenetBonusProperty
                    .FindPropertyRelative("affectType");
                SerializedProperty tenetTypeProperty = tenetBonusProperty
                    .FindPropertyRelative("tenetType");
                
                string affectString = ((AffectType) affectTypeProperty.enumValueIndex)
                    .ToString();
                string tenetTypeString = ((TenetType) tenetTypeProperty.enumValueIndex).ToString();

                string tenetBonusString = $"{affectString} {tenetTypeString}";
                tenetBonusProperty.FindPropertyRelative("name").stringValue = tenetBonusString;
                tenetBonusStringList.Add(tenetBonusString);
            }
            
            return $"BONUSED BY {string.Join(" and ", tenetBonusStringList)}";
        }

        private string ProcessTenetCostsDisplayName(SerializedProperty tenetCostsProperty)
        {
            if (tenetCostsProperty.arraySize > 0)
            {
                List<string> tenetCostStringList = new List<string>();
                    
                for (int i = 0; i < tenetCostsProperty.arraySize; i++)
                {
                    SerializedProperty tenetCostProperty = tenetCostsProperty.GetArrayElementAtIndex(i);
                    
                    SerializedProperty affectTypeProperty = tenetCostProperty
                        .FindPropertyRelative("affectType");
                    SerializedProperty tenetCostTypeProperty = tenetCostProperty
                        .FindPropertyRelative("tenetCostType");
                    SerializedProperty tenetTypeProperty = tenetCostProperty
                        .FindPropertyRelative("tenetType");

                    string affectString = ((AffectType) affectTypeProperty.enumValueIndex)
                        .ToString();
                    string tenetCostTypeString = ((TenetCostType) tenetCostTypeProperty.enumValueIndex)
                        .ToString();
                    string tenetString = ((TenetType) tenetTypeProperty.enumValueIndex)
                        .ToString();
                    
                    string costName = $"{affectString} {tenetCostTypeString} {tenetString}";
                    tenetCostProperty.FindPropertyRelative("name").stringValue = costName;
                    tenetCostStringList.Add(costName);
                    tenetCostProperty.serializedObject.ApplyModifiedProperties();
                }

                return $"IF {string.Join(" and ", tenetCostStringList)}";
            }

            return string.Empty;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            EditorGUI.GetPropertyHeight(property);
    }
}
