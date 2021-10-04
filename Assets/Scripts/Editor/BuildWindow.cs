using System;
using System.Collections.Generic;
using System.Linq;
using Game.Map;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public class BuildWindow : EditorWindow
    {
        private const string lastBuildVersionKey = "PDT.LastBuild.Version";
        private const string lastMapKey = "PDT.LastBuild.Map";
        
        [MenuItem("Window/Build Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<BuildWindow>();
            window.titleContent = new GUIContent("Build Tool");
            window.Show();
        }

        private void CreateGUI()
        {
            Label titleLabel = new Label("Build Tool")
            {
                style =
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    paddingBottom = 15,
                    paddingTop = 10,
                    paddingLeft = 15,
                    paddingRight = 15
                }
            };

            TextField versionField = new TextField("Version")
            {
                value = EditorPrefs.GetString(lastBuildVersionKey, "0.X.X") 
            };
            
            ObjectField objectField = new ObjectField("Map")
            {
                objectType = typeof(MapData),
                allowSceneObjects = false,
                value = AssetDatabase.LoadAssetAtPath<Object>(EditorPrefs.GetString(lastMapKey, ""))
            };

            Toggle devToggle = new Toggle("Development Build");

            var buildButtons = new VisualElement()
            {
                style =
                {
                    marginTop = 20,
                    marginBottom = 10,
                    marginLeft = 5,
                    marginRight = 5,
                    backgroundColor = new Color(0.26f, 0.26f, 0.26f),
                    alignItems = Align.Center,
                    paddingTop = 10,
                    paddingBottom = 10,
                    borderBottomLeftRadius = 10,
                    borderBottomRightRadius = 10,
                    borderTopLeftRadius = 10,
                    borderTopRightRadius = 10
                }
            };

            Toggle buildWindows = new Toggle("Build Windows");
            Toggle buildMac = new Toggle("Build Mac");
            Toggle buildLinux = new Toggle("Build Linux");

            buildButtons.Add(buildWindows);
            buildButtons.Add(buildMac);
            buildButtons.Add(buildLinux);


            buildButtons.Add(new Button(() =>
            {
                EditorPrefs.SetString(lastBuildVersionKey, versionField.value);
                EditorPrefs.SetString(lastMapKey, AssetDatabase.GetAssetPath(objectField.value));
                
                if (buildWindows.value)
                    BuildPlatform(versionField.value, BuildTarget.StandaloneWindows64, objectField.value as MapData, devToggle.value);
                
                if (buildMac.value)
                    BuildPlatform(versionField.value, BuildTarget.StandaloneOSX, objectField.value as MapData, devToggle.value);
                
                if (buildLinux.value)
                    BuildPlatform(versionField.value, BuildTarget.StandaloneLinux64, objectField.value as MapData, devToggle.value);
            })
            {
                text = "Build",
                style =
                {
                    marginTop = 8,
                    marginBottom = 8,
                    width = Length.Percent(50)
                }
            });

            Button openFolderButton = new Button(() => 
                    Application.OpenURL("file://" + Application.dataPath.Replace("Assets", "Builds")))
            {
                text = "Open Build Folder",
                style =
                {
                    width = Length.Percent(80),
                    marginLeft = StyleKeyword.Auto,
                    marginRight = StyleKeyword.Auto
                }
            };
            
            rootVisualElement.Add(titleLabel);
            rootVisualElement.Add(versionField);
            rootVisualElement.Add(objectField);
            rootVisualElement.Add(devToggle);
            rootVisualElement.Add(buildButtons);
            rootVisualElement.Add(openFolderButton);
        }

        private const string defaultMapPath =
            "Assets/ScriptableObjects/Maps/Rework Tutorial Map/Rework Tutorial Map.asset";
        
        /// <summary>
        /// Build game, useful for build automation and build scripts
        /// </summary>
        public static void BuildStandaloneWindows64()
        {
            BuildPlatform("DEV", BuildTarget.StandaloneWindows64, AssetDatabase.LoadAssetAtPath<MapData>(defaultMapPath));
        }
        
        /// <summary>
        /// Build game, useful for build automation and build scripts
        /// </summary>
        public static void BuildStandaloneOSX()
        {
            BuildPlatform("DEV", BuildTarget.StandaloneOSX, AssetDatabase.LoadAssetAtPath<MapData>(defaultMapPath));
        }
        
        /// <summary>
        /// Build game, useful for build automation and build scripts
        /// </summary>
        public static void BuildStandaloneLinux64()
        {
            BuildPlatform("DEV", BuildTarget.StandaloneLinux64, AssetDatabase.LoadAssetAtPath<MapData>(defaultMapPath));
        }

        /// <summary>
        /// Create a build.
        /// Kept as a static function so that we can make automated builds.
        /// </summary>
        public static void BuildPlatform(string version, BuildTarget buildTarget, MapData mapData, bool developmentBuild = false)
        {
            if (mapData == null)
            {
                Debug.LogError("Trying to build, but missing a map! Please specify a map!");
                return;
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetRequiredScenes(mapData).ToArray();
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.locationPathName = buildTarget == BuildTarget.StandaloneOSX
                ? $"{GetBuildPath(buildTarget)}-v{version}/Soul Searcher"
                : $"{GetBuildPath(buildTarget)}-v{version}/Soul Searcher-v{version}";
            buildPlayerOptions.options = developmentBuild 
                ? BuildOptions.Development | BuildOptions.AllowDebugging 
                : BuildOptions.None;

            // The output is not in .exe for windows. We need to make sure that happens
            if (buildTarget == BuildTarget.StandaloneWindows64)
                buildPlayerOptions.locationPathName += ".exe";

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
                Debug.Log("Build Succeeded: " + summary.totalSize / 1000000 + " megabytes");

            if (summary.result == BuildResult.Failed)
                Debug.LogError("Build Failed!");
        }

        private static IEnumerable<string> GetRequiredScenes(MapData mapData)
        {
            var encounterScenes = Enumerable.Empty<string>()
                .Concat(mapData.encounterNodes.SelectMany(n =>
                    n.EncounterData.GetAllPossibleScenes().Select(s => s.ScenePath))).Distinct()
                .Prepend(mapData.mainMenuScene);

            if (string.IsNullOrEmpty(mapData.mapScene.ScenePath))
            {
                return encounterScenes;
            }
            else
            {
                return encounterScenes
                    // Make sure this is first, so it loads in first
                    .Append(mapData.mapScene.ScenePath)
                    // We need this for some reason, otherwise exiting an encounter won't work
                    .Append("Assets/Scenes/Developer/Map Test.unity");
            }
        }

        /// <summary>
        /// Get an appropriate build location
        /// </summary>
        private static string GetBuildPath(BuildTarget buildTarget) => buildTarget switch
        {
            BuildTarget.StandaloneWindows64 => "Builds/SoulSearcher-WINDOWS",
            BuildTarget.StandaloneOSX => "Builds/SoulSearcher-MAC",
            BuildTarget.StandaloneLinux64 => "Builds/SoulSearcher-LINUX",
             _ => throw new ArgumentException($"Unsupported build target {buildTarget}")
        };
    }
}
