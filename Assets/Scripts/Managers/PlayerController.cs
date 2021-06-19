using Commands;
using UnityEngine;

namespace Managers
{
    public class PlayerController : MonoBehaviour
    {
        // TODO: Replace the following with a GridReadyCommand listener
        // NOTE: Uses Start() instead of Awake() so tilemap in GridController can set up
        private void Start()
        {
            // TODO: Obtain the number of players, their tenets and starting positions
            // Maybe do this through a level dictionary that contains these details?
            // For now placeholders will be used
            
            // TODO: This should reference a prefab instead of loading from resources
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerPlaceholder", typeof(GameObject));
            
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            playerManager.Spawn(playerPrefab, Vector2Int.zero);
            playerManager.Spawn(playerPrefab, Vector2Int.up);
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new PlayerUnitsReadyCommand());
        }
    }
}
