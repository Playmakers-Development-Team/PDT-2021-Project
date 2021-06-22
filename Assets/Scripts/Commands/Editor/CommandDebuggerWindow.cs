using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Managers;
using Units;
using UnityEditor;
using UnityEngine;

namespace Commands.Editor
{
    public class CommandDebuggerWindow : EditorWindow
    {
        private List<Type> commandTypes = new List<Type>();
        private Dictionary<Type, object[]> rememberedValues = new Dictionary<Type, object[]>();
        private Vector2 scrollPosition;
        
        [MenuItem("Window/Commands Debugger")]
        private static void ShowWindow()
        {
            var window = GetWindow<CommandDebuggerWindow>();
            window.titleContent = new GUIContent("Commands Debugger");
            window.Show();
        }

        private void OnEnable()
        {
            commandTypes = TypeCache.GetTypesDerivedFrom<Command>().ToList();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUIStyle titleStyle = EditorStyles.largeLabel;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;
            GUILayout.Label("Commands", titleStyle);
            
            EditorGUILayout.Space();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            foreach (Type commandType in commandTypes)
            {
                ConstructorInfo mainConstructor = commandType.GetConstructors().FirstOrDefault();

                if (mainConstructor != null)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(commandType.Name, EditorStyles.boldLabel);
                    var parameters = mainConstructor.GetParameters();
                    object[] parameterValues;

                    if (!rememberedValues.ContainsKey(commandType))
                    {
                        parameterValues = new object[parameters.Length];
                        rememberedValues[commandType] = parameterValues;
                    }
                    else
                    {
                        parameterValues = rememberedValues[commandType];
                    }

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        Type parameterType = parameters[i].ParameterType;
                        string parameterName = parameters[i].Name;
                        
                        if (parameterType == typeof(int))
                        {
                            parameterValues[i] ??= 0;
                            parameterValues[i] = EditorGUILayout.IntField(parameterName, (int) parameterValues[i]);
                        }
                        else if (parameterType == typeof(float))
                        {
                            parameterValues[i] ??= 0;
                            parameterValues[i] = EditorGUILayout.FloatField(parameterName, (float) parameterValues[i]);
                        }
                        else if (parameterType == typeof(string))
                        {
                            parameterValues[i] ??= string.Empty;
                            parameterValues[i] = EditorGUILayout.TextField(parameterName, (string) parameterValues[i]);
                        }
                        else if (parameterType == typeof(Vector2))
                        {
                            parameterValues[i] ??= new Vector2();
                            parameterValues[i] = EditorGUILayout.Vector2Field(parameterName, (Vector2) parameterValues[i]);
                        }
                        else if (parameterType == typeof(Vector2Int))
                        {
                            parameterValues[i] ??= new Vector2Int();
                            parameterValues[i] = EditorGUILayout.Vector2IntField(parameterName, (Vector2Int) parameterValues[i]);
                        }
                        else if (parameterType == typeof(Vector3))
                        {
                            parameterValues[i] ??= new Vector3();
                            parameterValues[i] = EditorGUILayout.Vector3Field(parameterName, (Vector3) parameterValues[i]);
                        }
                        else if (parameterType.IsAssignableFrom(typeof(IUnit)))
                        {
                            parameterValues[i] = EditorGUILayout.ObjectField(parameterName, (UnityEngine.Object) parameterValues[i], typeof(GameObject), true);
                            GameObject go = parameterValues[i] as GameObject;

                            if (go != null && go.TryGetComponent(out IUnit unit))
                            {
                                parameterValues[i] = unit;
                            }
                            else if (!(parameterValues[i] is IUnit))
                            {
                                parameterValues[i] = null;
                            }
                        }
                        else if (parameterType.IsAssignableFrom(typeof(Component)))
                        {
                            parameterValues[i] = EditorGUILayout.ObjectField(parameterName, (UnityEngine.Object) parameterValues[i], parameterType, true);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(8);
                    }

                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    
                    if (GUILayout.Button("Execute"))
                    {
                        Command command = (Command) mainConstructor.Invoke(parameterValues);

                        try
                        {
                            CommandManager commandManager = ManagerLocator.Get<CommandManager>();

                            if (commandManager == null)
                            {
                                Debug.Log(
                                    $"Command Debugger could not execute {commandType.Name}! {nameof(CommandManager)} not ready!");
                            }
                            else
                            {
                                commandManager.ExecuteCommand(command);
                                Debug.Log($"Command Debugger executed {commandType.Name}!");
                            }
                        }
                        catch (Exception err)
                        {
                            Debug.Log($"Command Debugger failed to execute command! \n{err}");
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.Space(5);
                }
            }
            
            GUILayout.EndScrollView();
        }
    }
}
