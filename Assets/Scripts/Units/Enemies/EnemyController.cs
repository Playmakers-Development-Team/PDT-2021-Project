using Commands;
using Grid;
using Grid.GridObjects;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Enemies
{
    public class EnemyController : UnitController<EnemyUnitData>
    {
        // Temporary debug buttons, likely to be removed later
        [SerializeField] private bool debugKillEnemyButton;
        [SerializeField] private bool debugDamagePlayerButton;

        private bool isSpawningEnemies = false;
        private int totalEnemies = 0; // Max is 203 at the moment -FRANCISCO: CAN CONFIRM IT DOES CRASH ABOVE 203

        // TODO: Use set enemy start positions as opposed to random positions later
        private GridManager gridManager;
        private GameObject enemyPrefab;

        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            gridManager = ManagerLocator.Get<GridManager>();
            unitManagerT = ManagerLocator.Get<EnemyManager>();
            
            #endregion
        }

        // NOTE: Uses Start() instead of Awake() so tilemap in GridController can set up
        protected override void Start()
        {
            if (totalEnemies <= 0)
            {
                base.Start();
                return;
            }
            
            // TODO: Is all of this still necessary?
            // TODO: Obtain the number of enemies, their tenets and starting positions
            // Maybe do this through a level dictionary that contains these details?
            // For now placeholders will be used

            // TODO: Replace with a GridReadyCommand listener
            isSpawningEnemies = true;

            unitManagerT.ClearUnits();

            commandManager.ExecuteCommand(new UnitManagerReadyCommand<EnemyUnitData>());
        }

        private void Update()
        {
            // NOTE: Spawning enemies is done in a loop as it spawning all enemies in start results
            // in race conditions with GridManager (i.e. enemies will continue to spawn in ignoring
            // spaces with enemies since they haven't been properly added to the grid yet)
            if (isSpawningEnemies)
            {
                if (unitManagerT.Units.Count >= totalEnemies)
                {
                    isSpawningEnemies = false;
                    commandManager.ExecuteCommand(new UnitsReadyCommand<EnemyUnitData>());
                }
            }
        }

        private void OnValidate()
        {
            if (debugKillEnemyButton)
                DebugKillEnemyFunction();

            if (debugDamagePlayerButton)
                DebugDamagePlayerButton();
        }

        private void SpawnEnemy()
        {
           IUnit enemyUnit = unitManagerT.Spawn(enemyPrefab, gridManager
           .GetRandomUnoccupiedCoordinates());

           enemyUnit.Name = enemyUnit.RandomizeName();

           // Debug.Log(enemyUnit.RandomizeName() + "RANDOMIZED");
           // Debug.Log(enemyUnit.Name);
        }

        // TODO: Refactor or remove.
        private void DebugKillEnemyFunction()
        {
            if (unitManagerT.Units.Count > 0)
                unitManagerT.Units[0].TakeDamageWithoutModifiers(1);

            debugKillEnemyButton = false;
        }

        // TODO: Refactor or remove.
        private void DebugDamagePlayerButton()
        {
            foreach (var enemy in unitManagerT.Units)
            {
                GridObject firstAdjacentPlayer = unitManagerT.FindAdjacentPlayer(enemy);
                if (firstAdjacentPlayer != null)
                {
                    if (firstAdjacentPlayer is IUnit firstAdjacentPlayerUnit)
                    {
                        // TODO: Get proper damage formula here
                        firstAdjacentPlayerUnit.TakeDamageWithoutModifiers(5);
                        debugDamagePlayerButton = false;
                        return;
                    }
                }
            }

            Debug.Log("No players adjacent to enemies found, no damage dealt");
            debugDamagePlayerButton = false;
        }
    }
}
