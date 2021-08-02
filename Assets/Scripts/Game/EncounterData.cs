using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class EncounterData : ScriptableObject
    {
        [field: SerializeField] public GameObject EncounterPrefab { get; set; }
    }
}
