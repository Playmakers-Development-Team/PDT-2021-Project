using GridObjects;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units
{
    public class EnemyController : MonoBehaviour
    {
        // NOTE: Uses Start() instead of Awake() so tilemap in GridController can set up
        private void Start()
        {
            // TODO: Obtain the number of enemies, their tenets and starting positions
            // Maybe do this through a level dictionary that contains these details?
            // For now placeholders will be used

            int enemyCount = 3;
            
            GameObject enemyPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/EnemyTemp", typeof(GameObject));

            EnemyManager enemyManager = ManagerLocator.Get<EnemyManager>();
            
            // TODO: Use set enemy start positions as opposed to random positions later
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            for (int i = 0; i < enemyCount; i++)
            {
                enemyManager.Spawn(enemyPrefab, gridManager.GetRandomCoordinates());
            }
        }
    }
}
