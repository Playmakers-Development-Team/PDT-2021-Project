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
                    commandManager.ExecuteCommand(new StatChangedCommand(iunit,
                        iunit.AttackStat.StatType, iunit.AttackStat.BaseValue,
                        iunit.AttackStat.Value, iunit.AttackStat.Value));
                    commandManager.ExecuteCommand(new StatChangedCommand(iunit,
                        iunit.DefenceStat.StatType, iunit.DefenceStat.BaseValue,
                        iunit.DefenceStat.Value, iunit.DefenceStat.Value));
                    commandManager.ExecuteCommand(new StatChangedCommand(iunit,
                        iunit.SpeedStat.StatType, iunit.SpeedStat.BaseValue,
                        iunit.SpeedStat.Value, iunit.SpeedStat.Value));
                    commandManager.ExecuteCommand(new StatChangedCommand(iunit,
                        iunit.MovementPoints.StatType, iunit.MovementPoints.BaseValue,
                        iunit.MovementPoints.Value, iunit.MovementPoints.Value));
                    commandManager.ExecuteCommand(new StatChangedCommand(iunit,
                        iunit.HealthStat.StatType, iunit.HealthStat.BaseValue,
                        iunit.HealthStat.Value, iunit.HealthStat.Value));
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
