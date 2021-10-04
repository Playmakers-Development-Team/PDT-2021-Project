using Commands;
using Game.Commands;
using Managers;
using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Event = UI.Core.Event;

namespace UI.MainMenu
{
    public class MainMenuDialogue : Dialogue
    {

        [SerializeField] private GameTitleComponent gameTitleComponent;
        [SerializeField] private CharacterImageComponent characterImageComponent;
        [SerializeField] private Transform mainMenuParent;
        [SerializeField] private GameObject exitConfirmationPrefab;
        [SerializeField] private GameObject MainMenuButtonPrefab;

        private GameObject exitConfirmedPrefabInstance;
        private GameObject mainMenuPrefabInstance;
        private CommandManager commandManager;


        #region Events

        internal readonly Event settingConfirmed = new Event();
        internal readonly Event settingsClosed = new Event();
        internal readonly Event creditsConfirmed = new Event();
        internal readonly Event creditsClosed = new Event();
        internal readonly Event gameStarted = new Event();
        internal readonly Event exitConfirmed = new Event();
        internal readonly Event exitStarted = new Event();
        internal readonly Event cancelExit = new Event();
        internal readonly Event buttonSelected = new Event();
        

        #endregion

        #region DialogueComponent
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        #endregion

        protected override void OnDialogueAwake()
        { 
            base.OnDialogueAwake();
            commandManager = ManagerLocator.Get<CommandManager>();
            mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
            characterImageComponent.RandomizeCharacterSprite();
            gameTitleComponent.UpdateTitle(characterImageComponent.GetCharacter());

            #region Listeners

            exitStarted.AddListener(() =>
            {
                exitConfirmedPrefabInstance = Instantiate(exitConfirmationPrefab, mainMenuParent);
                Destroy(mainMenuPrefabInstance);
            });

            exitConfirmed.AddListener(() =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });
            
            cancelExit.AddListener(() =>
            {
               mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
               Destroy(exitConfirmedPrefabInstance);
            });
            
            gameStarted.AddListener(() =>
            {
                commandManager.ExecuteCommand(new PlayGameCommand());
            });

            settingConfirmed.AddListener(() =>
            {
                //TODO: Open Settings Menu
            });
            
            settingsClosed.AddListener(() =>
            {
                //TODO: Close Settings Menu
            });
            
            creditsConfirmed.AddListener(() =>
            {
                //TODO: Open Credits Menu
            });
            
            creditsClosed.AddListener(() =>
            {
                //TODO: Close Credits Menu
            });
            
            #endregion
        }
        
        


    }
}
