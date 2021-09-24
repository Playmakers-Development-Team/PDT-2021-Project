using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Grid;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using Turn;
using UI.Core;
using Units;
using Units.Players;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Task = System.Threading.Tasks.Task;

namespace UI.Game.Grid
{
    public class GridUI : DialogueComponent<GameDialogue>
    {
        [SerializeField] private LayerMask clickLayer;
        
        [Header("Line of Sight Indicator")]
        
        [SerializeField] private LineRenderer line;

        // The following values are used to show mouse the position when selecting a movement tile
        private bool enableMouseHover;
        private Vector2Int hoveredCoordinate;
        
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Masking")]
        
        [SerializeField] private Color defaultColour;
        [SerializeField] private Color maskColour;
        [SerializeField] private float fadeDuration;
        
        [Header("Component References")]
        
        [SerializeField] private Tilemap tilemap;
        
        private GridManager gridManager;
        private TurnManager turnManager;
        private CommandManager commandManager;


        #region MonoBehaviour

        public void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame && !enableMouseHover
                || Camera.main == null)
                return;
            
            Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(-Camera.main.transform.forward, transform.position);

            if (!plane.Raycast(worldRay, out float distance) ||
                Physics.Raycast(worldRay, clickLayer))
                return;
            
            Vector2 worldPosition = worldRay.origin + worldRay.direction * distance;
            Vector2Int coordinate = gridManager.ConvertPositionToCoordinate(worldPosition);

            if (!gridManager.IsInBounds(coordinate))
                return;

            if (enableMouseHover)
            {
                hoveredCoordinate = coordinate;
                UpdateGrid();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
                TryMove(coordinate);
        }
        
        #endregion
        
        
        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }
        
        protected override void OnComponentEnabled()
        {
            base.OnComponentEnabled();
            commandManager.ListenCommand<GridObjectsReadyCommand>(OnGridObjectsReady);
        }

        protected override void OnComponentDisabled()
        {
            base.OnComponentDisabled();
            commandManager.ListenCommand<GridObjectsReadyCommand>(OnGridObjectsReady);
        }

        protected override void Subscribe()
        {
            dialogue.abilitySelected.AddListener(OnAbilitySelected);
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
            dialogue.abilityRotated.AddListener(OnAbilityRotated);
            dialogue.abilityConfirmed.AddListener(OnAbilityConfirmed);
            dialogue.modeChanged.AddListener(OnModeChanged);
        }

        protected override void Unsubscribe()
        {
            dialogue.abilitySelected.RemoveListener(OnAbilitySelected);
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityRotated.RemoveListener(OnAbilityRotated);
            dialogue.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
            dialogue.modeChanged.RemoveListener(OnModeChanged);
        }
        
        #endregion

        
        #region Listeners

        private void OnAbilitySelected(Ability ability)
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Aiming);
            UpdateGrid();
            UpdateLOSIndicator();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Default);
            UpdateGrid();
            UpdateLOSIndicator();
        }

        private void OnAbilityRotated(Vector2 direction)
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Aiming)
                return;
            
            
            UpdateGrid();
            UpdateLOSIndicator();
        }

        private void OnAbilityConfirmed()
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Default);
            UpdateGrid();
        }

        private void OnModeChanged(GameDialogue.Mode mode)
        {
            UpdateGrid();

            FadeObstacles(mode);
        }
        
        #endregion
        
        
        #region Drawing
        
        private void OnGridObjectsReady(GridObjectsReadyCommand cmd) => FillAll();

        private void UpdateGrid()
        {
            // TODO: Add IUnit.IsMoving check whenever that's implemented...
            
            FillAll();

            Vector2Int[] coordinates;
            switch (dialogue.DisplayMode)
            {
                case GameDialogue.Mode.Aiming when dialogue.SelectedAbility != null:
                    Vector2Int[] possibleCoordinates = dialogue.SelectedAbility.Shape.
                        GetPossibleCoordinates(turnManager.ActingUnit.Coordinate).
                        Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    
                    Fill(new GridSelection(possibleCoordinates, GridSelectionType.Valid));
                    
                    coordinates = dialogue.SelectedAbility.Shape.
                        GetHighlightedCoordinates(turnManager.ActingUnit.Coordinate, dialogue.AbilityDirection).
                        Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    
                    Fill(new GridSelection(coordinates, GridSelectionType.Selected));
                    
                    break;
                
                case GameDialogue.Mode.Moving when turnManager.ActingUnit.MovementPoints.Value > 0:
                    // Fill in the movable coordinates with GridSelectionType.Valid
                    coordinates = turnManager.ActingUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                    
                    //Fill in the blocked coordinates with GridSelectionType.Invalid
                    Vector2Int[] occupiedCoordinates = turnManager.ActingUnit.GetReachableOccupiedTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    Fill(new GridSelection(occupiedCoordinates, GridSelectionType.Invalid));

                    enableMouseHover = true;

                    if (coordinates.Contains(hoveredCoordinate))
                        Fill(new GridSelection(hoveredCoordinate, GridSelectionType.Selected));

                    break;
            }
        }

        private void UpdateLOSIndicator()
        {
            if (dialogue.SelectedAbility == null)
            {
                line.positionCount = 0;
                return;
            }
            
            Vector2Int[] coordinates = dialogue.SelectedAbility.Shape.
                GetHighlightedCoordinates(turnManager.ActingUnit.Coordinate,
                    dialogue.AbilityDirection).Where(v => gridManager.IsInBounds(v)).ToArray();

            if (!dialogue.SelectedAbility.Shape.ShouldShowLine || coordinates.Length == 0 ||
                !gridManager.GetGridObjectsByCoordinate(coordinates[0]).All(g => g is IUnit))
            {
                line.positionCount = 0;
                return;
            }

            // TODO: Implement...

            Vector2Int coordinate = coordinates[0];
            line.positionCount = 2;
            line.SetPositions(
            new Vector3[] {
                gridManager.ConvertCoordinateToPosition(coordinate),
                gridManager.ConvertCoordinateToPosition(turnManager.ActingUnit.Coordinate)
            });
        }

        private void Fill(GridSelection selection)
        {
            TileBase tile = GetTile(selection.Type);
            foreach (Vector2Int coordinate in selection.Spaces)
            {
                // BUG: The `tilemap != null` check prevents an error when loading a second
                // BUG: encounter. This needs to be checked.
                if (gridManager.GetGridObjectsByCoordinate(coordinate).All(g => g is IUnit) && tilemap != null)
                    tilemap.SetTile((Vector3Int) coordinate, tile);
            }
        }
        
        private void FillAll(GridSelectionType type = GridSelectionType.Default)
        {
            BoundsInt b = gridManager.LevelBoundsInt;
            List<Vector2Int> coordinates = new List<Vector2Int>();
            
            for (int x = b.xMin; x <= b.xMax; x++)
            {
                for (int y = b.yMin; y <= b.yMax; y++)
                {
                    if (gridManager.GetGridObjectsByCoordinate(new Vector2Int(x, y)).All(g => g is IUnit))
                        coordinates.Add(new Vector2Int(x, y));
                }
            }
            
            Fill(new GridSelection(coordinates.ToArray(), type));
        }

        private TileBase GetTile(GridSelectionType type)
        {
            return type switch
            {
                GridSelectionType.Default => defaultTile,
                GridSelectionType.Invalid => invalidTile,
                GridSelectionType.Valid => validTile,
                GridSelectionType.Selected => selectedTile,
                _ => null
            };
        }

        private void FadeObstacles(GameDialogue.Mode mode)
        {
            BoundsInt b = gridManager.LevelBoundsInt;
            
            for (int x = b.xMin; x <= b.xMax; x++)
            {
                for (int y = b.yMin; y <= b.yMax; y++)
                {
                    GridObject[] objs = gridManager.GetGridObjectsByCoordinate(new Vector2Int(x, y)).ToArray();
                    foreach (GridObject obj in objs)
                    {
                        if (!(obj is Obstacle obstacle) || !obstacle.Renderer)
                            continue;
                        
                        Color colour = mode == GameDialogue.Mode.Default ? defaultColour : maskColour;

                        FadeObstacle(obstacle, colour);
                    }
                }
            }
        }

        private async void FadeObstacle(Obstacle obstacle, Color colour)
        {
            Color startColour = obstacle.Renderer.material.color;
            float startTime = Time.time;

            while (Time.time <= startTime + fadeDuration)
            {
                if (!Application.isPlaying)
                    return;
                
                float t = (Time.time - startTime) / fadeDuration;
                obstacle.Renderer.material.color = Color.Lerp(startColour, colour, t);
                await Task.Yield();
            }
        }
        
        #endregion
        
        
        #region Movement
        
        // TODO: This would probably be better being in InputController...
        private void TryMove(Vector2Int destination)
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Moving)
                return;

            PlayerUnit playerUnit = turnManager.ActingPlayerUnit;
            
            // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
            List<Vector2Int> inRange = playerUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToList();
            
            if (!inRange.Contains(destination))
                return;

            enableMouseHover = false;
            
            dialogue.moveConfirmed.Invoke(new GameDialogue.MoveInfo(destination, dialogue.GetInfo(playerUnit)));
            dialogue.buttonSelected.Invoke();
            
            FillAll();
        }
        
        #endregion
    }
}
