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
                SerializedProperty defenceProperty = property.FindPropertyRelative("defenceValue");
                
                SerializedProperty nameProperty = property.FindPropertyRelative("name");
                SerializedProperty costsProperty = property.FindPropertyRelative("costs");

                if (costsProperty.arraySize == 0)
                {
                    nameProperty.stringValue = "Default";
                    // Damage and defence
                    if (damageProperty.intValue > 0 && defenceProperty.intValue > 0)
                        nameProperty.stringValue += $", {damageProperty.intValue} Damage and {defenceProperty.intValue} Defense ";
                    else if (damageProperty.intValue > 0)
                        nameProperty.stringValue += $", {damageProperty.intValue} Damage ";
                    else if (defenceProperty.intValue > 0)
                        nameProperty.stringValue += $", {defenceProperty.intValue} Defence ";
                    else
                        nameProperty.stringValue += ", nothing";
                }
                else
                {
                    // Damage and defence
                    if (damageProperty.intValue > 0 && defenceProperty.intValue > 0)
                        nameProperty.stringValue = $"{damageProperty.intValue} Damage and {defenceProperty.intValue} Defense ";
                    else if (damageProperty.intValue > 0)
                        nameProperty.stringValue = $"{damageProperty.intValue} Damage ";
                    else if (defenceProperty.intValue > 0)
                        nameProperty.stringValue = $"{defenceProperty.intValue} Defence ";
                    else
                        nameProperty.stringValue = "Nothing ";
                    
                    for (int i = 0; i < costsProperty.arraySize; i++)
                    {
                        if (i > 0)
                            nameProperty.stringValue += ", ";
                        
                        SerializedProperty costProperty = costsProperty.GetArrayElementAtIndex(i);
                        string costName = "";
                        
                        // Cost type
                        switch (costProperty.FindPropertyRelative("costType").enumValueIndex)
                        {
                            case 0:
                                nameProperty.stringValue += "with ";
                                costName += "With ";
                                break;
                            case 1:
                                nameProperty.stringValue += "per ";
                                costName += "Per ";
                                break;
                            case 2:
                                nameProperty.stringValue += "spend ";
                                costName += "Spend ";
                                break;
                        }

                        // Tenet
                        string tenet = ((TenetType) costProperty.FindPropertyRelative("tenet").enumValueIndex).ToString();
                        nameProperty.stringValue += tenet;
                        costName += tenet;

                        costProperty.FindPropertyRelative("name").stringValue = costName;
                        costProperty.serializedObject.ApplyModifiedProperties();
                    }
                }

                property.serializedObject.ApplyModifiedProperties();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
