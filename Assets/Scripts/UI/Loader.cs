using UnityEngine;

namespace UI
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private GameObject[] dialogues;

        private void Awake()
        {
            foreach (GameObject dialogue in dialogues)
                Instantiate(dialogue, transform);
        }
    }
}
