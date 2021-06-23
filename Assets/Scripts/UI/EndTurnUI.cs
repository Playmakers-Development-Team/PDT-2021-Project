using Commands;
using Managers;
using UnityEngine;

namespace UI
{
    public class EndTurnUI : MonoBehaviour
    {
        private CommandManager commandManager;
        private TurnManager turnManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private GridManager gridManager;

        private void Start()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
        }

        // BUG: Units are added to the end of the queue rather than being added based on their speed
        public void SpawnPlayer()
        {
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerPlaceholder", typeof(GameObject));

            playerManager.Spawn(playerPrefab, Vector2Int.left);
        }
        
        // BUG: Units are added to the end of the queue rather than being added based on their speed
        public void SpawnEnemy()
        {
            GameObject enemyPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/EnemyPlaceholder", typeof(GameObject));

            enemyManager.Spawn(enemyPrefab, gridManager.GetRandomUnoccupiedCoordinates());
        }
        
        public void EndTurn()
        {
            commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
        }
    }
}
