using Commands;
using Managers;
using Units;
using Units.Commands;
using Units.Players;
using Units.Stats;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(PlayerUnitData))]
    public class UnitDataDrawer: PropertyDrawer
    {
        private SerializedProperty health, movement, speed;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            var btnRect = new Rect(position.width / 4, position.y + 5, position.width / 2, 40);
            var defaultRect = new Rect(position.x + 5, position.y + 40, position.width, position.height);
            
            EditorGUI.PropertyField(defaultRect, property, label, true);
            speed = property.FindPropertyRelative("speed");

            if (GUI.Button(btnRect, "UPDATE UNIT STAT UI"))
            {
                if (Application.isPlaying)
                {
                    Debug.Log(speed);

                    CommandManager commandManager = ManagerLocator.Get<CommandManager>();
                    commandManager.ExecuteCommand(new SpeedChangedCommand((IUnit)property.serializedObject.targetObject, 1));
                }
                else
                {
                    Debug.Log("Cannot update unless the application is running");
                }
            }
            
            //base.OnGUI(position, property, label);
            
            // EditorGUI.BeginProperty(position, label, property);
            //
            // // Draw label
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            //
            // // Don't make child fields be indented
            // var indent = EditorGUI.indentLevel;
            // EditorGUI.indentLevel = 0;
            //
            //
            //
            // top = new Rect(position.x, position.y, position.width, position.height);
            // bottom = new Rect(position.x, position.y, position.width, position.height);
            //
            // var healthRect = new Rect(position.x, position.y + 20, position.width, position.height);
            // var moveRect = new Rect(position.x, position.y + 40, position.width, position.height);
            // var speedRect = new Rect(position.x, position.y + 60, position.width, position.height);
            //
            // //Find Property Relatives
            // health = property.FindPropertyRelative("healthPoints");
            // movement = property.FindPropertyRelative("movementPoints");
            // speed = property.FindPropertyRelative("speed");
            //
            // // EditorGUI.PropertyField(healthRect, health, GUIContent.none);
            // // EditorGUI.PropertyField(moveRect, movement, GUIContent.none);
            // // EditorGUI.PropertyField(speedRect, speed, GUIContent.none);
            //
            // // Set indent back to what it was
            // EditorGUI.indentLevel = indent;
            //
            // // if (GUI.Button(bottom, "Test"))
            // // {
            // //     Debug.Log("health");
            // // }
            // base.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + 400;
        }
    }
}
