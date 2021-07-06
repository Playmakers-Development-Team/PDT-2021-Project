using System.Collections.Generic;
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

                List<string> valueNameList = new List<string>();
                nameProperty.stringValue = string.Empty;
                
                bool hasValues = damageProperty.intValue != 0 
                                    || defenceProperty.intValue != 0
                                    || attackProperty.intValue != 0 
                                    || provideCountProperty.intValue != 0;
                
                if (costsProperty.arraySize == 0)
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
                
                if (costsProperty.arraySize != 0)
                {
                    nameProperty.stringValue += " if ";
                    
                    for (int i = 0; i < costsProperty.arraySize; i++)
                    {
                        if (i > 0)
                            nameProperty.stringValue += ", ";
                        
                        SerializedProperty costProperty = costsProperty.GetArrayElementAtIndex(i);
                        string costName = GetCostDisplayName(costProperty);
                        costProperty.FindPropertyRelative("name").stringValue = costName;
                        nameProperty.stringValue += costName;
                        costProperty.serializedObject.ApplyModifiedProperties();
                    }
                }

                property.serializedObject.ApplyModifiedProperties();
            }
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

                // Cost type name
                switch (tenetCost.FindPropertyRelative("tenetCostType").enumValueIndex)
                {
                    case 0:
                        tenetCostName += "With ";
                        break;
                    case 1:
                        tenetCostName += "Per ";
                        break;
                    case 2:
                        tenetCostName += "Spend ";
                        break;
                }

                // Tenet name
                string tenet = ((TenetType) tenetCost.FindPropertyRelative("tenetType").enumValueIndex).ToString();
                tenetCostName += tenet;

                tenetCostNameProperty.stringValue = tenetCostName;
                costName += tenetCostName;
                
                // If we have more than one tenet cost, add a comma
                if (i < tenetCost.arraySize - 1)
                    costName += ", ";
            }

            return costName;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            EditorGUI.GetPropertyHeight(property);
    }
}
