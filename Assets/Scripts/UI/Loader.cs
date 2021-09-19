using UnityEngine;

namespace UI
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private GameObject dialogue;

        private void Awake() => Instantiate(dialogue, transform);
    }
}
