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
                SerializedProperty costsProperty = property.FindPropertyRelative("costs");
                SerializedProperty bonusesProperty = property.FindPropertyRelative("bonuses");
                SerializedProperty keywordsProperty = property.FindPropertyRelative("keywords");

                List<string> valueNameList = new List<string>();
                nameProperty.stringValue = string.Empty;
                
                bool hasValues = damageProperty.intValue != 0 
                                    || defenceProperty.intValue != 0
                                    || attackProperty.intValue != 0 
                                    || provideCountProperty.intValue != 0;
                
                if (costsProperty.arraySize == 0 && keywordsProperty.arraySize == 0)
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

        private string ProcessCostsDisplayName(SerializedProperty costsProperty)
        {
            if (costsProperty.arraySize > 0)
            {
                List<string> costNameList = new List<string>();
                    
                for (int i = 0; i < costsProperty.arraySize; i++)
                {
                    SerializedProperty costProperty = costsProperty.GetArrayElementAtIndex(i);
                    string costName = GetCostDisplayName(costProperty);
                    costProperty.FindPropertyRelative("name").stringValue = costName;
                    costNameList.Add(costName);
                    costProperty.serializedObject.ApplyModifiedProperties();
                }

                return $"IF {string.Join(" and ", costNameList)}";
            }

            return string.Empty;
        }

        private string GetCostDisplayName(SerializedProperty costProperty)
        {
            string affectString = ((AffectType) costProperty.FindPropertyRelative("affectType").enumValueIndex).ToString();
            string costName = $"{affectString} ";
            SerializedProperty tenetCosts = costProperty.FindPropertyRelative("tenetCosts");

            for (int i = 0; i < tenetCosts.arraySize; i++)
            {
                string tenetCostName = string.Empty;
                SerializedProperty tenetCost = tenetCosts.GetArrayElementAtIndex(i);
                SerializedProperty tenetCostNameProperty = tenetCost.FindPropertyRelative("name");
                SerializedProperty tenetCostTypeProperty =
                    tenetCost.FindPropertyRelative("tenetCostType");
                SerializedProperty tenetTypeProperty = tenetCost.FindPropertyRelative("tenetType");
                
                string tenetCostTypeName = ((TenetCostType) tenetCostTypeProperty.enumValueIndex)
                    .ToString();
                string tenetName = ((TenetType) tenetTypeProperty.enumValueIndex).ToString();
                
                tenetCostName += $"{tenetCostTypeName} {tenetName}";

                tenetCostNameProperty.stringValue = tenetCostName;
                costName += tenetCostName;
                
                // If we have more than one tenet cost, add a comma
                if (i < tenetCosts.arraySize - 1)
                    costName += ", ";
            }

            return costName;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            EditorGUI.GetPropertyHeight(property);
    }
}
