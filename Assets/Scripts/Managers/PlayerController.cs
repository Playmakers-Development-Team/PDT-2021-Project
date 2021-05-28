using UnityEngine;

namespace Managers
{
    public class PlayerController : MonoBehaviour
    {
        // NOTE: Uses Start() instead of Awake() so tilemap in GridController can set up
        private void Start()
        {
            // TODO: Obtain the number of players, their tenets and starting positions
            // Maybe do this through a level dictionary that contains these details?
            // For now placeholders will be used
            
            GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/GridObjects/PlayerTemp", typeof(GameObject));
            Vector2Int startGridPosition = Vector2Int.zero;

            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            playerManager.Spawn(playerPrefab, startGridPosition);
        }
    }
}
