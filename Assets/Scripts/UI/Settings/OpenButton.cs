using UnityEngine;

namespace UI.Settings
{
    public class OpenButton : UIComponent<Dialogue>
    {
        [SerializeField] private GameObject dialoguePrefab;

        public void OnPressed()
        {
            Instantiate(dialoguePrefab, dialogue.transform.parent);
        }


        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }
    }
}
