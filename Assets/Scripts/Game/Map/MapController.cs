using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class MapController : MonoBehaviour
    {
        // TODO: Temporary serialisation to get MapData
        [SerializeField] private MapData mapData;
        [SerializeField] private GameObject encounterButtonPrefab;
        [SerializeField] private Transform mapCanvas;
        
        private GameManager gameManager;
        
        private void Start()
        {
            gameManager = ManagerLocator.Get<GameManager>();

            // TODO: Get MapData from the GameManager
            
            mapData.Initialise();

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
