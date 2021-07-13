using TMPro;
using UnityEngine;

namespace UI.Test
{
    public class TestTextComponent : UIComponent<TestDialogue>
    {
        [SerializeField] private TextMeshProUGUI text;


        protected override void Subscribe()
        {
            text.text = dialogue.gameObject.name;
        }

        protected override void Unsubscribe()
        {
        }
    }
}
