using Commands;
using Managers;
using UnityEngine;

namespace UI
{
    public class EndTurnUI : MonoBehaviour
    {
        private CommandManager commandManager;
        private TurnManager turnManager;
        private GridManager gridManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;

        private void Start()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
        }

        public void SpawnPlayer()
        {
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerPlaceholder", typeof(GameObject));
            playerManager.Spawn(playerPrefab, Vector2Int.left);
        }
        
        public void SpawnEnemy()
        {
            GameObject enemyPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/EnemyPlaceholder", typeof(GameObject));
            enemyManager.Spawn(enemyPrefab, gridManager.GetRandomUnoccupiedCoordinates());
        }
        
        public void EndTurn() => commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
    }
}
