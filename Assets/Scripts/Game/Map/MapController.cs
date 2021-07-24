using Commands;
using Game.Commands;
using Managers;
using UnityEngine;

namespace Game.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapData mapData;
        
        private GameManager gameManager;
        private CommandManager commandManager;

        private void Awake()
        {
            gameManager = ManagerLocator.Get<GameManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }
        
        private void Start() {
            // Uses the mapData stored in GameManager where possible. Serialised mapData can still
            // be used for testing purposes.
            if (gameManager.CurrentMapData != null)
                mapData = gameManager.CurrentMapData;
            else
            {
                gameManager.CurrentMapData = mapData;
                
                // TODO: Make sure mapData is still initialised if loaded from gameManager.
                mapData.Initialise();
            }
            
            commandManager.ExecuteCommand(new MapReadyCommand(mapData));
        }
    }
}
