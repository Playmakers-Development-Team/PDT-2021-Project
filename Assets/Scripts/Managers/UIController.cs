using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Cysharp.Threading.Tasks;
using GridObjects;
using UI;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace Managers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private Transform abilityParent;
        [SerializeField] private GameObject audioPanel;

        private GridManager gridManager;
        private UIManager uiManager;
        private CommandManager commandManager;
        private UnitManager unitManager;
        private PlayerManager playerManager;

        /// <summary>
        /// Stores the current actingunit.
        /// </summary>
        private PlayerUnit actingUnit => unitManager.ActingPlayerUnit;

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

        private List<Vector2Int> selectedMoveRange;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            uiManager = ManagerLocator.Get<UIManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();

            commandManager.ListenCommand<TurnQueueCreatedCommand>(cmd =>
            {
                timelineIsReady = true;

                if (actingUnit == null)
                    return;

                abilityIndex = 0;
                UpdateAbilityUI((PlayerUnit) actingUnit);
            });
        }

        private void Start()
        {
            commandManager.ListenCommand<StartTurnCommand>(cmd =>
            {
                if (unitManager.ActingUnit is EnemyUnit)
                    ClearAbilityUI();

                uiManager.ClearAbilityHighlight();

                if (unitManager.ActingUnit is PlayerUnit)
                {
                    abilityIndex = 0;
                    UpdateAbilityUI((PlayerUnit) actingUnit);
                }
            });

            commandManager.ListenCommand<EndTurnCommand>(cmd =>
            {
                if (unitManager.ActingUnit is EnemyUnit)
                    ClearAbilityUI();

                uiManager.ClearAbilityHighlight();

                if (unitManager.ActingUnit is PlayerUnit)
                {
                    abilityIndex = 0;
                    UpdateAbilityUI((PlayerUnit) actingUnit);
                }
            });
        }

        /// <summary>
        /// Updated the ability ui cards for the new active unit
        /// </summary>
        /// <param name="the current active unit"></param>
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
            uiManager.HighlightAbility(((GridObject) unit).Coordinate,
                ((OrdinalDirection) UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(OrdinalDirection)).Length)).ToVector2(), ability);
        }

        private void Update()
        {
            if (
                Input.GetKeyDown(KeyCode.
                    E)) // SELECTS THE ABILITY PRESSING E MULTIPLE TIMES WILL GO THROUGH THE ABILITY LIST
            {
                if (ManagerLocator.Get<PlayerManager>().WaitForDeath)
                    return; //can be more efficient
                if (actingUnit == null)
                    return;

                if (isUnselected)
                {
                    isUnselected = false;
                    UpdateAbilityUI((PlayerUnit) actingUnit);
                }

                if (abilityIndex >= abilityCards.Count)
                    abilityIndex = 0;

                if (abilityIndex != 0)
                    abilityCards[abilityIndex - 1].UnHighlightAbility();

                abilityCards[abilityIndex].HighlightAbility();
                actingUnit.CurrentlySelectedAbility = abilityCards[abilityIndex].Ability;
                TestAbilityHighlight(actingUnit, actingUnit.CurrentlySelectedAbility);

                abilityIndex++;
            }

            if (Input.GetKeyDown(KeyCode.Q)) //DESELECTS ABILITIES
            {
                if (actingUnit == null)
                    return;

                isUnselected = false;
                actingUnit.CurrentlySelectedAbility = null;
                uiManager.ClearAbilityHighlight();
                abilityIndex = 0;
            }

            if (Input.GetKeyDown(KeyCode.M)) // SELECTS MOVEMENT
            {
                if (unitManager.ActingUnit == null || unitManager.ActingUnit is EnemyUnit)
                    return;

                if (unitManager.ActingUnit == ManagerLocator.Get<TurnManager>().CurrentUnit)
                {
                    nextClickWillMove = true;
                    Debug.Log("Next click will move.");

                    UpdateMoveRange(gridManager.GetAllReachableTiles(
                        ((GridObject) unitManager.ActingUnit).Coordinate,
                        (int) unitManager.ActingUnit.MovementActionPoints.Value));
                }
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
            Vector2Int gridPos = GetCoordinateFromClick();

            foreach (IUnit unit in playerManager.PlayerUnits)
            {
                if (unit is PlayerUnit playerUnit)
                {
                    if (gridManager.ConvertPositionToCoordinate(playerUnit.transform.position) ==
                        gridPos)
                    {
                        playerManager.SelectUnit(playerUnit);
                        return;
                    }
                }
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

        //TODO Move this to its 

        private void MoveUnit()
        {
            //if (ManagerLocator.Get<PlayerManager>().WaitForDeath) return; //can be more efficient
            Vector2Int gridPos = GetCoordinateFromClick();

            // Check if tile is unoccupied
            //This cannot be checked with move range as no occupied tile will be added to it
            //This only needs to be kept if a different thing happens if the player selects an occupied space
            if (gridManager.GetTileDataByCoordinate(gridPos).GridObjects.Count != 0)
            {
                Debug.Log("Target tile is occupied.");
                return;
            }

            if (!selectedMoveRange.Contains(gridPos))
            {
                Debug.Log("Target tile out of range.");
                return;
            }

            IUnit playerUnit = unitManager.ActingPlayerUnit;

            //playerUnit.MovementActionPoints.Value -= selectedMoveRange.Count;

            Debug.Log(((GridObject) playerUnit).Coordinate + " to " + gridPos + " selected");
            List<GridObject> gridUnit =
                gridManager.GetGridObjectsByCoordinate(((GridObject) playerUnit).Coordinate);

            // playerUnit.MovementActionPoints.Value = Math.Min(0,
            //     playerUnit.MovementActionPoints.Value -=
            //         ManhattanDistance.GetManhattanDistance(((GridObject) playerUnit).Coordinate,
            //             gridPos));

            var moveCommand = new MoveCommand(playerUnit, gridPos);

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
            if (actingUnit == null || actingUnit.CurrentlySelectedAbility == null)
                return;

            Vector2 mouseVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                                   actingUnit.transform.position);
            Vector2 castVector = Quaternion.AngleAxis(-45f, Vector3.forward) * mouseVector;

            if (Input.GetKeyDown(KeyCode.A))
            {
                isCastingAbility = !isCastingAbility;
            }

            if (isCastingAbility)
            {
                uiManager.HighlightAbility(actingUnit.Coordinate, castVector,
                    actingUnit.CurrentlySelectedAbility);
            }

            if (isCastingAbility && Input.GetMouseButtonDown(1))
            {
                actingUnit.CurrentlySelectedAbility.Use(actingUnit, actingUnit.Coordinate,
                    castVector);

                actingUnit.ChangeAnimation(PlayerUnit.AnimationStates.Casting);

                float flag = 0;
                while (flag < actingUnit.animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    flag += Time.deltaTime;
                    await UniTask.Yield();
                }

                actingUnit.ChangeAnimation(PlayerUnit.AnimationStates.Idle);
            }
        }
    }
}
