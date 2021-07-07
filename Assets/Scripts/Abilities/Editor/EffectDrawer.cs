using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Conditionals;
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
                SerializedProperty bonusesProperty = property.FindPropertyRelative("bonuses");
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
                {
                    nameProperty.stringValue += "Default, ";
                }

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
                {
                    valueNameList.Add("Nothing");
                }

                nameProperty.stringValue += string.Join(" and ", valueNameList);

                
                if (bonusesProperty.arraySize != 0)
                    nameProperty.stringValue += " " + ProcessBonusesDisplayName(bonusesProperty);
                
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

        private string ProcessBonusesDisplayName(SerializedProperty bonusesProperty)
        {
            if (bonusesProperty.arraySize > 0)
            {
                List<string> bonusesNameList = new List<string>();

                for (int i = 0; i < bonusesProperty.arraySize; i++)
                {
                    SerializedProperty bonusProperty = bonusesProperty.GetArrayElementAtIndex(i);
                    string bonusName = GetBonusDisplayName(bonusProperty);
                    bonusProperty.FindPropertyRelative("name").stringValue = bonusName;
                    bonusesProperty.serializedObject.ApplyModifiedProperties();
                    bonusesNameList.Add(bonusName);
                }
                
                return $"BONUSED BY {string.Join(" and ", bonusesNameList)}";
            }

            return string.Empty;
        }

        private string GetBonusDisplayName(SerializedProperty bonusProperty)
        {
            string affectString = ((AffectType) bonusProperty.FindPropertyRelative("affectType").enumValueIndex).ToString();
            SerializedProperty perTenetProperty = bonusProperty.FindPropertyRelative("perTenet");
            List<string> tenetNameList = new List<string>();

            for (int i = 0; i < perTenetProperty.arraySize; i++)
            {
                SerializedProperty tenetProperty = perTenetProperty.GetArrayElementAtIndex(i);
                string tenetName = ((TenetType) tenetProperty.enumValueIndex).ToString();
                tenetNameList.Add(tenetName);
            }
            
            return $"{affectString} {string.Join(", ", tenetNameList)}";
        }

        private string ProcessTenetCostsDisplayName(SerializedProperty tenetCostsProperty)
        {
            if (tenetCostsProperty.arraySize > 0)
            {
                List<string> tenetCostNameList = new List<string>();
                    
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
                    tenetCostNameList.Add(costName);
                    tenetCostProperty.serializedObject.ApplyModifiedProperties();
                }

                return $"IF {string.Join(" and ", tenetCostNameList)}";
            }

            return string.Empty;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            EditorGUI.GetPropertyHeight(property);
    }
}
