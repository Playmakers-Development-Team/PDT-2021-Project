using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private MapData mapData;
        [SerializeField] private GameObject encounterButtonPrefab;
        [SerializeField] private Transform mapCanvas;
        
        private GameManager gameManager;
        
        private void Start()
        {
            gameManager = ManagerLocator.Get<GameManager>();

            // Use the mapData stored in GameManager where possible. Serialised mapData can still be
            // used for testing purposes.
            if (gameManager.CurrentMapData != null)
                mapData = gameManager.CurrentMapData;
            else
            {
                gameManager.CurrentMapData = mapData;
                
                // TODO: Make sure mapData is still initialised if loaded from gameManager.
                mapData.Initialise();
            }

            DisplayMap();
        }

        private void DisplayMap()
        {
            foreach (var encounterNode in mapData.encounterNodes)
            {
                var encounterButton = Instantiate(encounterButtonPrefab, mapCanvas);

                encounterButton.GetComponentInChildren<Text>().text = encounterNode.ToString();

                encounterButton.GetComponent<Button>().onClick.AddListener(() => LoadEncounter(encounterNode));
            }
        }

        private void LoadEncounter(EncounterNode encounterNode)
        {
            gameManager.ToEncounter(encounterNode.EncounterData);
        }
    }
}
