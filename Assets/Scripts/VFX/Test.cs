using UnityEngine;
using UnityEngine.InputSystem;

namespace VFX
{
    // TODO: Remove...
    public class Test : MonoBehaviour
    {
        [SerializeField] private GameObject[] scenarios;

        private int index;

        private void Start()
        {
            for (int i = 0; i < scenarios.Length; i++)
                scenarios[i].SetActive(i == index);
        }

        private void Update()
        {
            if (!Keyboard.current.backspaceKey.wasPressedThisFrame)
                return;
            
            scenarios[index].SetActive(false);
            index = (index + 1) % scenarios.Length;
            scenarios[index].SetActive(true);
        }
    }
}
