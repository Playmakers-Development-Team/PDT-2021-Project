using Commands;
using Game.Commands;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    // TODO: Remove once world map UI has been implemented.
    public class TempMapUI : MonoBehaviour
    {
        [SerializeField] private GameObject encounterButtonPrefab;
        [SerializeField] private Transform mapCanvas;

        private CommandManager commandManager;
        private GameManager gameManager;

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            gameManager = ManagerLocator.Get<GameManager>();

            commandManager.ListenCommand<MapReadyCommand>(cmd => DisplayMap(cmd.MapData));
            gameManager.SetEndEncounterToLoadMap();
        }

        private void DisplayMap(MapData mapData)
        {
            foreach (var encounterNode in mapData.encounterNodes)
            {
                var encounterButton = Instantiate(encounterButtonPrefab, mapCanvas);

                encounterButton.GetComponentInChildren<Text>().text = encounterNode.ToString();

                encounterButton.GetComponent<Button>().onClick.AddListener(() => gameManager.LoadEncounter(encounterNode.EncounterData));
            }
        }
    }
}
