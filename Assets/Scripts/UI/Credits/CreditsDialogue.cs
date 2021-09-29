using UI.Core;
using UnityEngine;

namespace UI.Credits
{
    public class CreditsDialogue : Dialogue
    {
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        // TODO: Not sure if this is the standard for getting input.
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                manager.Pop();
        }
    }
}
