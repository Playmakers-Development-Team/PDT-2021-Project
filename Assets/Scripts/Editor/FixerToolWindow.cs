using System.IO;
using System.Linq;
using System.Reflection;
using AI;
using Grid.GridObjects;
using Turn;
using UI.Game;
using Units;
using Units.Enemies;
using Units.Players;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class FixerToolWindow : UnityEditor.EditorWindow
    {
        [MenuItem("Window/Fixer Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<FixerToolWindow>();
            window.titleContent = new GUIContent("Fixer Tool");
            window.Show();
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(new Button(OnFixSceneClicked)
            {
                text = "APPLY EVERY FIX WITH THE SCENE",
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 50,
                    marginLeft = 30,
                    marginRight = 30,
                    marginTop = 20,
                }
            });
            
            rootVisualElement.Add(new Button(OnFixVFX)
            {
                text = "Fix VFX (Adds Unit Controller)",
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            });
            
            rootVisualElement.Add(new Button(OnFixUnitStructure)
            {
                text = "Fix to new Unit Prefab Structure",
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            });
            
            rootVisualElement.Add(new Button(OnFixToGameLoader)
            {
                text = "Fix Game Dialogue to Game Loader",
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            });

            rootVisualElement.Add(new Button(OnFixMusicClicked)
            {
                text = "Fix Music",
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            });
            
            rootVisualElement.Add(new Button(OnFixPinkBackground)
            {
                text = "Fix background being Pink",
                style =
                {
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10,
                    marginTop = 10
                }
            });
        }

        private void OnFixMusicClicked()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            GameObject wiseGlobal = GameObject.Find("WwiseGlobal");

            if (wiseGlobal != null)
            {
                foreach (AkEvent akEvent in wiseGlobal.GetComponents<AkEvent>())
                    DestroyImmediate(akEvent);
                
                DestroyImmediate(wiseGlobal.GetComponent<AkBank>());
                DestroyImmediate(wiseGlobal.GetComponent<AkGameObj>());
            }

            if (!GameObject.Find("Music"))
            {
                var musicPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Audio/Music.prefab");
                PrefabUtility.InstantiatePrefab(musicPrefab);
            }
        }
        
        private void OnFixVFX()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            const string unitControllerPath = "Assets/Prefabs/VFX/GridObject VFX Controller.prefab";

            if (!GameObject.Find("--- VFX ---"))
            {
                GameObject vfx = new GameObject("--- VFX ---");
                GameObject vfxControllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(unitControllerPath);
                GameObject vfxController = (GameObject) PrefabUtility.InstantiatePrefab(vfxControllerPrefab, vfx.transform);
            }
            
            Debug.Log("VFX fixing Successful! Unit controller is added!");
        }

        private void OnFixUnitStructure()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            ReplaceAllUnits();
            
            Debug.Log("Unit fixing Successful!");
        }

        private void OnFixToGameLoader()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            ReplaceGameDialogue();
            
            Debug.Log("Game loader fixing Successful!");
        }

        private void OnFixSceneClicked()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            OnFixToGameLoader();
            OnFixUnitStructure();
            OnFixPinkBackground();
            OnFixMusicClicked();
            
            Debug.Log("Scene fixing Successful!");
        }

        private void OnFixPinkBackground()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            GameObject updatedGridObjectPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Background/Mountain Trail Grid.prefab");
            
            
            GameObject backgroundGameObject = GameObject.Find("--- BACKGROUND SYSTEM ---");
            UnityEngine.Grid gridObject = backgroundGameObject.GetComponentInChildren<UnityEngine.Grid>();

            if (gridObject == null || gridObject.name != "Mountain Trail Grid")
            {
                if (gridObject != null)
                    Undo.DestroyObjectImmediate(gridObject.gameObject);
                GameObject updatedGridObject = (GameObject)PrefabUtility
                    .InstantiatePrefab(updatedGridObjectPrefab, backgroundGameObject.transform);
                Undo.RecordObject(updatedGridObject, "Add in updated grid background");
            }
        }

        private void ReplaceGameDialogue()
        {
            string fixedName = "Game Loader";
            
            if (!GameObject.Find(fixedName))
            {
                GameDialogue oldDialogue = GameObject.FindObjectOfType<GameDialogue>();
                GameObject updatedPrefab = AssetDatabase
                    .LoadAssetAtPath<GameObject>($"Assets/Prefabs/UI/Game Loader.prefab");
            
                string updatedPrefabStatus = updatedPrefab != null ? "FOUND" : "NOT FOUND";
                Debug.Log($"Game Dialogue {updatedPrefabStatus}");
                GameObject newDialogue = (GameObject) PrefabUtility.InstantiatePrefab(updatedPrefab, oldDialogue.transform.parent);
                DestroyImmediate(oldDialogue.gameObject);

                // var gridCanvas = newDialogue.transform.Find("Grid UI/Canvas").GetComponent<Canvas>();
                // var unitsCanvas = newDialogue.transform.Find("Units Canvas").GetComponent<Canvas>();
                //
                // Camera mainCamera = Camera.main;
                // gridCanvas.worldCamera = mainCamera;
                // unitsCanvas.worldCamera = mainCamera;
            }
        }

        private void ReplaceAllUnits(bool keepParent = true)
        {
            // Change the path here.
            const string baseGridObjectsPath = "Assets/Prefabs/Grid Objects";
            var units = GameObject.FindObjectsOfType<GridObject>();

            foreach (var gridObject in units)
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(gridObject.gameObject))
                {
                    Debug.LogWarning($"Skipping {gridObject.name}, it doesn't need to be replaced");
                    continue;
                }
                
                IUnit oldIUnit = gridObject.GetComponent<IUnit>();
                Obstacle oldObstacle = gridObject.GetComponent<Obstacle>();
                PlayerUnit oldPlayerUnit = gridObject.GetComponent<PlayerUnit>();
                EnemyUnit oldEnemyUnit = gridObject.GetComponent<EnemyUnit>();
                EnemyMeleeAi oldMeleeAi = gridObject.GetComponent<EnemyMeleeAi>();
                EnemyRangedAi oldRangedAi = gridObject.GetComponent<EnemyRangedAi>();
                EnemySpawnerAi oldEnemySpawnerAi = gridObject.GetComponent<EnemySpawnerAi>();
                
                string oldPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gridObject.gameObject);
                
                string prefabFileName = Path.GetFileName(oldPath);
                string prefabFileNameWithoutExtension = prefabFileName.Replace(".prefab", "");
                string newPath;
                string filePath;

                if (oldPlayerUnit != null)
                {
                    newPath = $"{baseGridObjectsPath}/Player Units/{prefabFileNameWithoutExtension}/";
                    filePath = newPath + prefabFileName;
                }
                else if (oldMeleeAi != null)
                {
                    newPath = $"{baseGridObjectsPath}/Enemy Units/Mouscle/";
                    filePath = newPath + "Mouscle.prefab";
                }
                else if (oldRangedAi != null)
                {
                    newPath = $"{baseGridObjectsPath}/Enemy Units/Stag of Grief/";
                    filePath = newPath + "Stag of Grief.prefab";
                }
                else if (oldEnemySpawnerAi != null)
                {
                    newPath = $"{baseGridObjectsPath}/Enemy Units/Melee Spawner/";
                    filePath = newPath + "Melee Spawner.prefab";
                }
                else if (oldObstacle != null)
                {
                    newPath = $"{baseGridObjectsPath}/Obstacles/Boulder 2a/";
                    filePath = newPath + "Boulder 2a.prefab";
                }
                else
                {
                    Debug.LogWarning($"Cannot replace grid object {gridObject.name}, no equivalent prefab found!");
                    continue;
                }

                // Check if we need to fix it
                // If the IUnit name has logic, then we are already up to date.
                if (!prefabFileName.Contains("Logic") && !oldPath.Contains(newPath))
                {
                    GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                    GameObject newGameObject;

                    if (keepParent)
                    {
                        newGameObject = (GameObject) PrefabUtility.InstantiatePrefab(newPrefab, gridObject.transform.parent);
                    }
                    else
                    {
                        newGameObject = (GameObject) PrefabUtility.InstantiatePrefab(newPrefab);
                    }

                    // Undo.RegisterCreatedObjectUndo(newGameObject, $"Fixed {newGameObject.name}");
                    GridObject newGridObject = newGameObject.GetComponentInChildren<GridObject>();
                    IUnit newIUnit = newGameObject.GetComponentInChildren<IUnit>();
                    EnemyMeleeAi newMeleeAi = newGameObject.GetComponentInChildren<EnemyMeleeAi>();
                    EnemyRangedAi newRangedAi = newGameObject.GetComponentInChildren<EnemyRangedAi>();
                
                    newGameObject.transform.position = gridObject.transform.position;
                    newGameObject.name = gridObject.name;

                    if (oldIUnit != null && newIUnit != null)
                    {
                        if (newIUnit is PlayerUnit newPlayerUnit)
                            EditorUtility.CopySerialized(oldPlayerUnit, newPlayerUnit);

                        if (newIUnit is EnemyUnit newEnemyUnit)
                            EditorUtility.CopySerialized(oldEnemyUnit, newEnemyUnit);
                    }

                    if (oldMeleeAi != null)
                        EditorUtility.CopySerialized(oldMeleeAi, newMeleeAi);

                    if (oldRangedAi != null)
                        EditorUtility.CopySerialized(oldRangedAi, newRangedAi);

                    ReplaceGridObjectInTurn(gridObject, newGridObject);
                    // Replace it
                    DestroyImmediate(gridObject.gameObject);
                }
            }
        }

        private void ReplaceGridObjectInTurn(GridObject oldObject, GridObject newObject)
        {
            TurnController turnController = GameObject.FindObjectOfType<TurnController>();
            SerializedObject serializedObject = new SerializedObject(turnController);
            var preMadeTimelineProperty = serializedObject.FindProperty("preMadeTimeline");

            if (preMadeTimelineProperty != null)
            {
                // Copy over into a new array
                for (int i = 0; i < preMadeTimelineProperty.arraySize; i++)
                {
                    SerializedProperty gridObjectProperty = preMadeTimelineProperty.GetArrayElementAtIndex(i);

                    if (gridObjectProperty.objectReferenceValue == oldObject.gameObject)
                        gridObjectProperty.objectReferenceValue = newObject.gameObject;
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
