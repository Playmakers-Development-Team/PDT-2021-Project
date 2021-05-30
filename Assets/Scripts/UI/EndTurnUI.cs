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

        public void SpawnPlayer()
        {
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerTemp", typeof(GameObject));

            playerManager.Spawn(playerPrefab, gridManager.GetRandomUnoccupiedCoordinates());
        }
        
        public void SpawnEnemy()
        {
            GameObject enemyPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/EnemyTemp", typeof(GameObject));

            enemyManager.Spawn(enemyPrefab, gridManager.GetRandomUnoccupiedCoordinates());
        }

        
        public void EndTurn()
        {
            commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
        }
        
        
    }
}
