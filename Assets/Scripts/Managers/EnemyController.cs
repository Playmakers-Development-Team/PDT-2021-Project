using Commands;
using Cysharp.Threading.Tasks;
using GridObjects;
using Units;
using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class EnemyController : MonoBehaviour
    {
        // Temporary debug buttons, likely to be removed later
        [SerializeField] private bool debugKillEnemyButton = false;
        [SerializeField] private bool debugDamagePlayerButton = false;
        
        private bool isSpawningEnemies = false;
        private int totalEnemies = 10; // Max is 203 at the moment -FRANCISCO: CAN CONFIRM IT DOES CRASH ABOVE 203 
        
        // TODO: Use set enemy start positions as opposed to random positions later
        private GridManager gridManager;
        private EnemyManager enemyManager;
        private CommandManager commandManager;
        private UnitManager unitManager;
        private GameObject enemyPrefab;

        // NOTE: Uses Start() instead of Awake() so tilemap in GridController can set up
        private void Start()
        {
            // TODO: Obtain the number of enemies, their tenets and starting positions
            // Maybe do this through a level dictionary that contains these details?
            // For now placeholders will be used

            gridManager = ManagerLocator.Get<GridManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();

            enemyPrefab =
                (GameObject) Resources.Load("Prefabs/GridObjects/EnemyPlaceholder", typeof(GameObject));
            
            // TODO: Replace with a GridReadyCommand listener
            isSpawningEnemies = true;
        }

        private void Update()
        {
            // NOTE: Spawning enemies is done in a loop as it spawning all enemies in start results
            // in race conditions with GridManager (i.e. enemies will continue to spawn in ignoring
            // spaces with enemies since they haven't been properly added to the grid yet)
            if (isSpawningEnemies)
            {
                if (enemyManager.EnemyUnits.Count < totalEnemies)
                    SpawnEnemy();
                else
                {
                    isSpawningEnemies = false;
                    ManagerLocator.Get<CommandManager>().ExecuteCommand(new EnemyUnitsReadyCommand());
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
           IUnit enemyunit = enemyManager.Spawn(enemyPrefab, gridManager
           .GetRandomUnoccupiedCoordinates());

           enemyunit.Name = enemyunit.RandomizeName();
           
           Debug.Log(enemyunit.RandomizeName() + "RANDOMZIED");
           Debug.Log(enemyunit.Name);
           
        }
        
        private void DebugKillEnemyFunction()
        {
            if (enemyManager.EnemyUnits.Count > 0)
                enemyManager.EnemyUnits[0].TakeDamage(1);
            
            debugKillEnemyButton = false;
        }
        
        private void DebugDamagePlayerButton()
        {
            foreach (var enemy in enemyManager.EnemyUnits)
            {
                GridObject firstAdjacentPlayer = enemyManager.FindAdjacentPlayer(enemy);
                if (firstAdjacentPlayer != null)
                {
                    if (firstAdjacentPlayer is IUnit firstAdjacentPlayerUnit)
                    {
                        // TODO: Get proper damage formula here
                        firstAdjacentPlayerUnit.TakeDamage(5);
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
