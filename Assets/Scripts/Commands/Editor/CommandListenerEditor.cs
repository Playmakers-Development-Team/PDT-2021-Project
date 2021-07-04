using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Commands.Editor
{
    [CustomEditor(typeof(CommandListener))]
    public class CommandListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty commandTypeProperty;
        private GenericMenu commandsMenu = new GenericMenu();

        private Type CommandType => TypeCache.GetTypesDerivedFrom<Command>()
            .FirstOrDefault(t => t.Name == commandTypeProperty.stringValue);

        private void OnEnable()
        {
            commandTypeProperty = serializedObject.FindProperty("commandTypeName");
            var commandTypes = TypeCache.GetTypesDerivedFrom<Command>();
            commandsMenu = new GenericMenu();
            
            foreach (var commandType in commandTypes)
            {
                bool isEnabled = commandTypeProperty.stringValue == commandType.AssemblyQualifiedName;
                commandsMenu.AddItem(new GUIContent(commandType.Name), isEnabled, () =>
                {
                    commandTypeProperty.stringValue = commandType.Name;
                    serializedObject.ApplyModifiedProperties();
                });
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Command");
            string commandName = CommandType != null ? CommandType.Name : "None";
            EditorGUILayout.LabelField(commandName, EditorStyles.textField);
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Select Command"))
            {
                commandsMenu.ShowAsContext();
            }
        }
    }
}
