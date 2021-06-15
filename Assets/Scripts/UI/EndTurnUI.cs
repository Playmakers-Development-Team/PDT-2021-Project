using Commands;
using Managers;
using UnityEngine;

namespace UI
{
    public class EndTurnUI : MonoBehaviour
    {
        private CommandManager commandManager;
        private TurnManager turnManager;
        private UnitManager unitManager;
        private GridManager gridManager;

        private void Start()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            gridManager = ManagerLocator.Get<GridManager>();
        }

        public void SpawnPlayer()
        {
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerPlaceholder", typeof(GameObject));

            unitManager.Spawn(playerPrefab, Vector2Int.left);
        }
        
        public void SpawnEnemy()
        {
            GameObject enemyPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/EnemyPlaceholder", typeof(GameObject));

            unitManager.Spawn(enemyPrefab, gridManager.GetRandomUnoccupiedCoordinates());
        }
        
        public void EndTurn() => commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
        
    }
}
