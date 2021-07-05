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

        if (gameManager.CurrentEncounterData != null)
            encounterData = gameManager.CurrentEncounterData;

        Instantiate(encounterData.encounterPrefab);
    }
}
