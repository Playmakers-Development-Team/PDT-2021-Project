using Commands;
using Managers;
using Units;
using Units.Commands;
using Units.Players;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(PlayerUnitData))]
    public class PlayerUnitDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var btnRect = new Rect(position.width / 4, position.y + 5, position.width / 2, 40);
            var defaultRect = new Rect(position.x + 5, position.y + 40, position.width,
                position.height);

            EditorGUI.PropertyField(defaultRect, property, label, true);

            if (GUI.Button(btnRect, "UPDATE UNIT STAT UI"))
            {
                if (Application.isPlaying)
                {
                    IUnit iunit = (IUnit) property.serializedObject.targetObject;
                    CommandManager commandManager = ManagerLocator.Get<CommandManager>();
                    commandManager.ExecuteCommand(
                        new AttackChangeCommand(iunit, (int) iunit.Attack.BaseAdder));
                    commandManager.ExecuteCommand(
                        new MovementActionPointChangedCommand(iunit,
                            iunit.MovementActionPoints.BaseValue));
                    commandManager.ExecuteCommand(
                        new SpeedChangedCommand(iunit, iunit.Speed.BaseValue));
                }
                else
                {
                    Debug.Log("Cannot update unless the application is running");
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + 540;
        }
    }
}
