using System;
using System.Collections.Generic;
using Abilities;
using Commands;
using Cysharp.Threading.Tasks;
using GridObjects;
using Tiles;
using UI;
using Units;
using Units.Commands;
using UnityEngine;
using Utility;

namespace Managers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private Transform abilityParent;
        [SerializeField] private GameObject abilityUIPreviewPrefab;
        [SerializeField] private Transform abilityPreviewParent;

        [SerializeField] private GameObject audioPanel;
        [SerializeField] private LineRenderer abilityLineRenderer;
        [SerializeField] private bool showGridLines = true;
        [SerializeField] private LineRenderer gridLinePrefab;

        private GridManager gridManager;
        private UIManager uiManager;
        private CommandManager commandManager;
        private UnitManager unitManager;
        private PlayerManager playerManager;
        private TurnManager turnManager;

        /// <summary>
        /// The player unit whose turn it currently is.
        /// </summary>
        private PlayerUnit actingPlayerUnit => turnManager.ActingPlayerUnit;

        private List<AbilityCard> abilityCardsPreviews = new List<AbilityCard>();

        /// <summary>
        /// A list of ability cards showing the units current abilities
        /// </summary>
        private List<AbilityCard> abilityCards = new List<AbilityCard>();

        /// <summary>
        /// A check to determine if the timeline is ready
        /// </summary>
        private bool timelineIsReady;

        /// <summary>
        /// A check to determine if the ability has been unselected
        /// </summary>
        private bool isUnselected;

        /// <summary>
        /// The current index of the current acting player unit 
        /// </summary>
        private int abilityIndex;

        private bool nextClickWillMove = false;

        private bool isCastingAbility;

        public bool printMoveRangeCoords = false;

        private bool canCastAbility = true;
        

        private List<Vector2Int> selectedMoveRange;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            uiManager = ManagerLocator.Get<UIManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            turnManager = ManagerLocator.Get<TurnManager>();

            commandManager.ListenCommand<TurnQueueCreatedCommand>(cmd =>
            {
                timelineIsReady = true;

                if (actingPlayerUnit == null)
                    return;

                abilityIndex = 0;
                UpdateAbilityUI(actingPlayerUnit);
            });

            uiManager.abilityLineRenderer = abilityLineRenderer;
        }

        private void Start()
        {
            commandManager.ListenCommand<StartTurnCommand>(cmd =>
            {
                if (turnManager.ActingUnit is EnemyUnit)
                    ClearAbilityUI();

                uiManager.ClearAbilityHighlight();

                if (turnManager.ActingUnit is PlayerUnit)
                {
                    abilityIndex = 0;
                    UpdateAbilityUI(actingPlayerUnit);
                }
                
                canCastAbility = true;
            });

            commandManager.ListenCommand<EndTurnCommand>(cmd =>
            {
                if (turnManager.ActingUnit is EnemyUnit)
                    ClearAbilityUI();

                uiManager.ClearAbilityHighlight();

                if (turnManager.ActingUnit is PlayerUnit)
                {
                    abilityIndex = 0;
                    UpdateAbilityUI(actingPlayerUnit);
                }
            });
            
            commandManager.ListenCommand<UnitSelectedCommand>(cmd =>
            {

                if (unitManager.SelectedUnit is EnemyUnit)
                    return;
                
                RemoveAbilitiesPreview();
                
                foreach (var ability in unitManager.SelectedUnit.Abilities)
                    AddAbilitiesPreview(ability);
                
            });
            
            commandManager.ListenCommand<UnitDeselectedCommand>(cmd =>
            {
                RemoveAbilitiesPreview();
            });
            
            GenerateGridLines();
        }

        private void GenerateGridLines()
        {
            if (!showGridLines)
                return;
            
            gridManager = ManagerLocator.Get<GridManager>();
            Vector2Int levelBounds = gridManager.LevelBounds;
            int startX = -levelBounds.x / 2 - 1;
            int endX = levelBounds.x / 2;
            int startY = -levelBounds.y / 2 - 1;
            int endY = levelBounds.y / 2;
            
            for (int x = startX; x <= endX; x++)
            {
                Vector2Int startCoor = new Vector2Int(x, startY);
                Vector2Int endCoor = new Vector2Int(x, endY);
                
                CreateGridLine(gridManager.ConvertCoordinateToPosition(startCoor),
                    gridManager.ConvertCoordinateToPosition(endCoor));
            }

            for (int y = startY; y <= endY; y++)
            {
                Vector2Int startCoor = new Vector2Int(startX, y);
                Vector2Int endCoor = new Vector2Int(endX, y);

                CreateGridLine(gridManager.ConvertCoordinateToPosition(startCoor),
                    gridManager.ConvertCoordinateToPosition(endCoor));
            }
        }

        private void CreateGridLine(Vector2 startPos, Vector2 endPos)
        {
            LineRenderer gridLine = Instantiate(gridLinePrefab, transform);
            gridLine.positionCount = 2;
            gridLine.SetPositions(new Vector3[] { startPos, endPos });
        }

        private void AddAbilitiesPreview(Ability ability)
        {
            var abilityCardObject = Instantiate(abilityUIPreviewPrefab, abilityPreviewParent);
            var abilityCard = abilityCardObject.GetComponent<AbilityCard>();
            abilityCard.SetAbility(ability);
            abilityCardsPreviews.Add(abilityCard);
        }

        private void RemoveAbilitiesPreview()
        {
            foreach (var abilityCard in abilityCardsPreviews) // updated formatting to fit convention
                Destroy(abilityCard.gameObject);

            abilityCardsPreviews.Clear();
        }

        /// <summary>
        /// Updated the ability ui cards for the new active unit
        /// </summary>
        /// <param name="unit">The current acting unit.</param>
        private void UpdateAbilityUI(PlayerUnit unit)
        {
            ClearAbilityUI();

            if (unit is null)
            {
                Debug.LogWarning(
                    "GridController.UpdateAbilityUI should not be passed a null value. Use GridController.ClearAbilityUI instead.");
                return;
            }

            foreach (var ability in unit.Abilities)
                AddAbilityField(ability);
            
            
        }

        /// <summary>
        /// Clears the ability cards used when a player unit is deselected
        /// </summary>
        private void ClearAbilityUI()
        {
            foreach (var abilityCard in abilityCards) // updated formatting to fit convention
                Destroy(abilityCard.gameObject);

            abilityCards.Clear();
        }

        /// <summary>
        /// Instantiates the ability cards
        /// </summary>
        private void AddAbilityField(Ability ability)
        {
            var abilityCardObject = Instantiate(abilityUIPrefab, abilityParent);
            var abilityCard = abilityCardObject.GetComponent<AbilityCard>();
            abilityCard.SetAbility(ability);
            abilityCards.Add(abilityCard);
            
            
        }

        private void TestAbilityHighlight(IUnit unit, Ability ability)
        {
            uiManager.HighlightAbility(unit.Coordinate,
                ((OrdinalDirection) UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(OrdinalDirection)).Length)).ToVector2(), ability);
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                unitManager.DeselectUnit();
            }
            // SELECTS THE ABILITY PRESSING E MULTIPLE TIMES WILL GO THROUGH THE ABILITY LIST
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                if (ManagerLocator.Get<PlayerManager>().WaitForDeath) return; //can be more efficient
                if (actingPlayerUnit == null)
                    return;

                if (isUnselected)
                {
                    isUnselected = false;
                    UpdateAbilityUI(actingPlayerUnit);
                }

                if (abilityIndex >= abilityCards.Count)
                    abilityIndex = 0;


                for (int i = 0; i < abilityCards.Count; i++)
                {
                    if (i != abilityIndex)
                    {
                        abilityCards[i].UnHighlightAbility();
                    }
                    else
                    {
                        abilityCards[i].HighlightAbility();
                    }
                }

                actingPlayerUnit.CurrentlySelectedAbility = abilityCards[abilityIndex].Ability;
                abilityIndex++;
            }

            if (Input.GetKeyDown(KeyCode.Q)) //DESELECTS ABILITIES
            {
                if (actingPlayerUnit == null)
                    return;
                
                for (int i = 0; i < abilityCards.Count; i++)
                    abilityCards[i].UnHighlightAbility();
                

                isUnselected = false;
                actingPlayerUnit.CurrentlySelectedAbility = null;
                uiManager.ClearAbilityHighlight();
                abilityIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.M)) // SELECTS MOVEMENT
            {
                if (turnManager.ActingUnit == null || turnManager.ActingUnit is EnemyUnit)
                    return;
                
                nextClickWillMove = true;
                Debug.Log("Next click will move.");
                
                nextClickWillMove = true;
                Debug.Log("Next click will move.");

                UpdateMoveRange(gridManager.GetAllReachableTiles(
                    turnManager.ActingUnit.Coordinate,
                    (int) turnManager.ActingUnit.MovementActionPoints.Value));
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                audioPanel.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (nextClickWillMove)
                {
                    nextClickWillMove = false;
                    MoveUnit();
                    selectedMoveRange.Clear();
                    UpdateMoveRange(selectedMoveRange);
                }
                else
                {
                    SelectUnit();
                }
            }

            HandleAbilityCasting();
        }

        private void SelectUnit()
        {
            Vector2Int coord = GetCoordinateFromClick();

            var gridObjects = gridManager.GetGridObjectsByCoordinate(coord);
            
            if (gridObjects.Count > 0)
            {
                if (gridObjects[0] is IUnit unit)
                {
                    unitManager.SelectUnit(unit);
                    return;
                }

                Debug.Log("Cannot select non-unit GridObjects.");
            }

            playerManager.DeselectUnit();
        }

        private void UpdateMoveRange(List<Vector2Int> moveRange)
        {
            selectedMoveRange = moveRange;
            //Any Grid highlighting updates should go here
            if (printMoveRangeCoords)
            {
                foreach (var coord in moveRange)
                {
                    Debug.Log(coord);
                }
            }
        }
        
        private void MoveUnit()
        {
            //if (ManagerLocator.Get<PlayerManager>().WaitForDeath) return; //can be more efficient
            Vector2Int gridPos = GetCoordinateFromClick();

            TileData tileData = gridManager.GetTileDataByCoordinate(gridPos);

            if (tileData is null)
            {
                Debug.Log("No tile data at this location. Unit was not moved.");
                return;
            }

            // Check if tile is unoccupied
            // This cannot be checked with move range as no occupied tile will be added to it
            // This only needs to be kept if a different thing happens if the player selects an occupied space
            if (tileData.GridObjects.Count != 0)
            {
                Debug.Log("Target tile is occupied. Unit was not moved.");
                return;
            }

            if (!selectedMoveRange.Contains(gridPos))
            {
                Debug.Log("Target tile out of range. Unit was not moved.");
                return;
            }
            
            IUnit playerUnit = turnManager.ActingPlayerUnit;
            
            //playerUnit.MovementActionPoints.Value -= selectedMoveRange.Count;
          
            Debug.Log(playerUnit.Coordinate + " to " + gridPos + " selected");
            List<GridObject> gridUnit = gridManager.GetGridObjectsByCoordinate(playerUnit.Coordinate);
            
            // playerUnit.MovementActionPoints.Value = Math.Min(0,
            //     playerUnit.MovementActionPoints.Value -=
            //         ManhattanDistance.GetManhattanDistance(playerUnit.Coordinate, gridPos));
            
            var moveCommand = new StartMoveCommand(playerUnit, gridPos);
            commandManager.ExecuteCommand(moveCommand);
        }

        private Vector2Int GetCoordinateFromClick()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // TODO look into this later, put the subtraction somewhere better
            return gridManager.ConvertPositionToCoordinate(mousePos) + new Vector2Int(1, 1);
        }

        private async void HandleAbilityCasting()
        {
            if (actingPlayerUnit == null || actingPlayerUnit.CurrentlySelectedAbility == null)
                return;

            Vector2 mouseVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                                   actingPlayerUnit.transform.position);
            
            Vector2 castVector = Quaternion.AngleAxis(-45f, Vector3.forward) * mouseVector;

            if (Input.GetKeyDown(KeyCode.A) && canCastAbility)
            {
                isCastingAbility = !isCastingAbility;
            }

            if (isCastingAbility)
            {
                uiManager.HighlightAbility(actingPlayerUnit.Coordinate, castVector, actingPlayerUnit.CurrentlySelectedAbility);
            }

            if (isCastingAbility && Input.GetMouseButtonDown(1))
            {
                isCastingAbility = false;
                canCastAbility = false;
                commandManager.ExecuteCommand(new AbilityCommand(actingPlayerUnit, castVector, actingPlayerUnit.CurrentlySelectedAbility));
                uiManager.ClearAbilityHighlight();
                ClearAbilityUI();
                
                // actingPlayerUnit.CurrentlySelectedAbility.Use(actingPlayerUnit, actingPlayerUnit.Coordinate,
                //     castVector);

                actingPlayerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Casting);

                float flag = 0;
                while (flag < actingPlayerUnit.animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    flag += Time.deltaTime;
                    await UniTask.Yield();
                }

                actingPlayerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Idle);
            }
        }
    }
}
