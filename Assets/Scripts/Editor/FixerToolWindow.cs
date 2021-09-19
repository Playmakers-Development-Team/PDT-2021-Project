using System.Linq;
using System.Reflection;
using AI;
using Grid.GridObjects;
using Turn;
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
                text = "Fix Scene",
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

        private void OnFixSceneClicked()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            ReplaceGameDialogue();
            ReplaceAllUnits();
            
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
            string fixedName = "Game Dialogue (Second Iteration)";
            
            if (!GameObject.Find(fixedName))
            {
                GameObject oldDialogue = GameObject.Find("Game Dialogue");
                GameObject updatedPrefab = AssetDatabase
                    .LoadAssetAtPath<GameObject>($"Assets/Prefabs/UI/SecondIteration/{fixedName}.prefab");
            
                string updatedPrefabStatus = updatedPrefab != null ? "FOUND" : "NOT FOUND";
                Debug.Log($"Game Dialogue {updatedPrefabStatus}");
                GameObject newDialogue = (GameObject) PrefabUtility.InstantiatePrefab(updatedPrefab, oldDialogue.transform.parent);
                DestroyImmediate(oldDialogue);

                var gridCanvas = newDialogue.transform.Find("Grid UI/Canvas").GetComponent<Canvas>();
                var unitsCanvas = newDialogue.transform.Find("Units Canvas").GetComponent<Canvas>();

                Camera mainCamera = Camera.main;
                gridCanvas.worldCamera = mainCamera;
                unitsCanvas.worldCamera = mainCamera;
            }
        }

        private void ReplaceAllUnits()
        {
            var units = GameObject.FindObjectsOfType<GridObject>();

            foreach (var gridObject in units)
            {
                IUnit oldIUnit = gridObject.GetComponent<IUnit>();
                EnemyMeleeAi oldMeleeAi = gridObject.GetComponent<EnemyMeleeAi>();
                EnemyRangedAi oldRangedAi = gridObject.GetComponent<EnemyRangedAi>();
                
                string oldPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gridObject.gameObject);
                const string fixedPath = "Assets/Prefabs/Units/Fixed/Fixed/";

                // Check if we need to fix it
                if (!oldPath.Contains(fixedPath))
                {
                    string newPath = oldPath.Replace("Assets/Prefabs/Units/", fixedPath);
                    GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);
                
                    GameObject newGridObject = (GameObject) PrefabUtility
                        .InstantiatePrefab(newPrefab, gridObject.transform.parent);
                    IUnit newIUnit = newGridObject.GetComponent<IUnit>();
                    EnemyMeleeAi newMeleeAi = newGridObject.GetComponent<EnemyMeleeAi>();
                    EnemyRangedAi newRangedAi = newGridObject.GetComponent<EnemyRangedAi>();
                
                    newGridObject.transform.position = gridObject.transform.position;
                    newGridObject.name = gridObject.name;

                    if (oldIUnit != null && newIUnit != null)
                    {
                        if (oldIUnit is PlayerUnit oldPlayerUnit && newIUnit is PlayerUnit newPlayerUnit)
                            EditorUtility.CopySerialized(oldPlayerUnit, newPlayerUnit);

                        if (oldIUnit is EnemyUnit oldEnemyUnit && newIUnit is EnemyUnit newEnemyUnit)
                            EditorUtility.CopySerialized(oldEnemyUnit, newEnemyUnit);
                    }

                    if (oldMeleeAi != null)
                        EditorUtility.CopySerialized(oldMeleeAi, newMeleeAi);

                    if (oldRangedAi != null)
                        EditorUtility.CopySerialized(oldRangedAi, newRangedAi);

                    ReplaceGridObjectInTurn(gridObject, newGridObject.GetComponent<GridObject>());
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
