using Game;
using Managers;
using UnityEngine;

public class EncounterController : MonoBehaviour
{
    // TODO: Temporary serialisation to get EncounterData
    [SerializeField] private EncounterData encounterData;
    
    private GameManager gameManager;

    private void Start()
    {
        gameManager = ManagerLocator.Get<GameManager>();
        
        // TODO: Get EncounterData from the GameManager

        Instantiate(encounterData.encounterPrefab);
    }
}
