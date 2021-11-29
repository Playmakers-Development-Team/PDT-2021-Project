using Audio.Commands;
using Commands;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.InputSystem;

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
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CommandManager commandManager = ManagerLocator.Get<CommandManager>();
                commandManager.ExecuteCommand(new PostSound("Stop_Credits_Theme"));
                commandManager.ExecuteCommand(new PostSound("Play_Opening_Theme"));
                
                manager.Pop();
            }
                
        }
    }
}
