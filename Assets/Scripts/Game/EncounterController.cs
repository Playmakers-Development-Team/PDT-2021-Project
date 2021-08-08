using Managers;
using UnityEngine;

namespace Game
{
    public class EncounterController : MonoBehaviour
    {
        [SerializeField] private EncounterData encounterData;
    
        private GameManager gameManager;

        private void Start()
        {
            gameManager = ManagerLocator.Get<GameManager>();
        
            // Uses the encounterData stored in GameManager where possible. Serialised encounterData
            // can still be used for testing purposes.
            if (gameManager.CurrentEncounterData != null)
                encounterData = gameManager.CurrentEncounterData;
            else
                gameManager.CurrentEncounterData = encounterData;

            Instantiate(encounterData.EncounterPrefab);
        }
    }
}
