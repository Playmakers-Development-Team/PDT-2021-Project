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
                SerializedProperty costsProperty = property.FindPropertyRelative("costs");
                
                List<string> valueNameList = new List<string>();
                nameProperty.stringValue = string.Empty;
                
                bool hasValues = damageProperty.intValue != 0 
                                    || defenceProperty.intValue != 0
                                    || attackProperty.intValue != 0 
                                    || provideCountProperty.intValue != 0;
                
                if (costsProperty.arraySize == 0 && keywordsProperty.arraySize == 0)
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
                
                if (costsProperty.arraySize != 0)
                    nameProperty.stringValue += " " + ProcessCostsDisplayName(costsProperty);
                
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

        private string ProcessCostsDisplayName(SerializedProperty costsProperty)
        {
            if (costsProperty.arraySize <= 0)
                return string.Empty;
            
            List<string> tenetCostStringList = new List<string>();
                
            for (int i = 0; i < costsProperty.arraySize; i++)
            {
                SerializedProperty compositeCostProperty = costsProperty.GetArrayElementAtIndex(i);
                // This name is auto generated by the CompositeCostDrawer
                string name = compositeCostProperty.FindPropertyRelative("name").stringValue;
                tenetCostStringList.Add(name);
            }

            return $"IF {string.Join(" and ", tenetCostStringList)}";
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            EditorGUI.GetPropertyHeight(property);
    }
}
