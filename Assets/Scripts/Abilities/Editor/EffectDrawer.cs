using System;
using System.Collections.Generic;
using System.Linq;
using TenetStatuses;
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
                SerializedProperty affectTargetProperty = property.FindPropertyRelative("affectTargets");
                SerializedProperty affectUserProperty = property.FindPropertyRelative("affectUser");
                
                SerializedProperty damageProperty = property.FindPropertyRelative("damageValue");
                SerializedProperty directDamageProperty = property.FindPropertyRelative("directDamage");
                SerializedProperty attackProperty = property.FindPropertyRelative("attackValue");
                SerializedProperty defenceProperty = property.FindPropertyRelative("defenceValue");
                SerializedProperty attackForEncounterProperty =
                    property.FindPropertyRelative("attackForEncounter");
                SerializedProperty defenceForEncounterProperty =
                    property.FindPropertyRelative("defenceForEncounter");
                SerializedProperty provideProperty = property.FindPropertyRelative("providingTenet");
                SerializedProperty provideTenetTypeProperty = provideProperty.FindPropertyRelative("tenetType");
                SerializedProperty provideCountProperty = provideProperty.FindPropertyRelative("stackCount");
                
                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                SerializedProperty bonusesProperty = property.FindPropertyRelative("bonuses");
                SerializedProperty keywordsProperty = property.FindPropertyRelative("keywords");
                SerializedProperty costsProperty = property.FindPropertyRelative("costs");
                
                List<string> valueNameList = new List<string>();
                nameProperty.stringValue = string.Empty;

                bool hasValues = damageProperty.intValue != 0
                                 || defenceProperty.intValue != 0
                                 || attackProperty.intValue != 0
                                 || provideCountProperty.intValue != 0
                                 || attackForEncounterProperty.intValue != 0
                                 || defenceForEncounterProperty.intValue != 0;
                
                // Affect types
                bool affectUser = affectUserProperty.boolValue;
                bool affectTarget = affectTargetProperty.boolValue;
                List<string> affects = new List<string>();
                
                if (affectUser)
                    affects.Add("User");
                
                if (affectTarget)
                    affects.Add("Target");

                if (affectUser || affectTarget)
                    nameProperty.stringValue += string.Join(" and ", affects.ToArray());
                else
                    nameProperty.stringValue += "Disabled";

                nameProperty.stringValue += " ➤ ";
                
                // If there is no conditions, it should say default
                if (costsProperty.arraySize == 0 && keywordsProperty.arraySize == 0)
                    nameProperty.stringValue += "Default, ";

                // Damage and defence
                if (hasValues)
                {
                    string directDamageString = directDamageProperty.boolValue ? " Directly" : string.Empty;
                    
                    if (damageProperty.intValue != 0)
                        valueNameList.Add($"{damageProperty.intValue} Damage{directDamageString}");
                    
                    if (attackProperty.intValue != 0)
                        valueNameList.Add($"{attackProperty.intValue} Attack");
                    
                    if (attackForEncounterProperty.intValue != 0)
                        valueNameList.Add($"{attackForEncounterProperty.intValue} Attack for the encounter");
                    
                    if (defenceForEncounterProperty.intValue != 0)
                        valueNameList.Add($"{defenceForEncounterProperty.intValue} Defence for the encounter");
                    
                    if (defenceProperty.intValue != 0)
                        valueNameList.Add($"{defenceProperty.intValue} Defence");
                    
                    if (provideCountProperty.intValue != 0)
                        valueNameList.Add($"Provide {provideCountProperty.intValue} {(TenetType)provideTenetTypeProperty.enumValueIndex}");
                }
                
                if (!hasValues)
                    valueNameList.Add("Nothing");
                
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
                    
                    if (keywordProperty.objectReferenceValue != null)
                        keywordNameList.Add(keywordProperty.objectReferenceValue.name);
                }

                return $"WITH {string.Join(" and ", keywordNameList)}";
            }
            
            return string.Empty;
        }

        private string ProcessBonusesDisplayName(SerializedProperty bonusesProperty)
        {
            List<string> bonusStringList = new List<string>();

            for (int i = 0; i < bonusesProperty.arraySize; i++)
            {
                SerializedProperty compositeBonusProperty = bonusesProperty.GetArrayElementAtIndex(i);
                // This name is auto generated by the CompositeBonusDrawer
                string name = compositeBonusProperty.FindPropertyRelative("name").stringValue;
                bonusStringList.Add(name);
            }
            
            return $"BONUSED BY {string.Join(" and ", bonusStringList)}";
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
