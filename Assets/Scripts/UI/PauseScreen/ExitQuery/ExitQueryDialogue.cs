using Commands;
using Game;
using Game.Commands;
using Managers;
using UI.Core;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.PauseScreen.ExitQuery
{
    public class ExitQueryDialogue : Dialogue
    {
        // TODO: Add to the right regions
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}
        
        internal readonly Event buttonSelected = new Event();
        internal readonly Event cancelExit = new Event();
        internal readonly Event exitToDesktop = new Event();
        internal readonly Event exitToMainMenu = new Event();

        private CommandManager commandManager;
        
        protected override void OnDialogueAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            
            cancelExit.AddListener(() =>
            {
                manager.Pop();
            });

            exitToDesktop.AddListener(() =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            exitToMainMenu.AddListener(() =>
            {
                commandManager.ExecuteCommand(new MainMenuCommand());
            });
        }
    }
}
