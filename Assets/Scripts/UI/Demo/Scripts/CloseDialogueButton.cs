using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Demo.Scripts
{
    public class CloseDialogueButton : DialogueComponent<Dialogue>
    {
        [SerializeField] private Button button;

        
        protected override void Subscribe() => button.onClick.AddListener(OnClick);

        protected override void Unsubscribe() => button.onClick.RemoveListener(OnClick);

        private void OnClick() => manager.Pop();
    }
}
