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
        [SerializeField] private GameObject settingsMenuPrefab;
        [SerializeField] private GameObject exitConfirmationPrefab;
        [SerializeField] private GameObject creditsDialoguePrefab;


        private CommandManager commandManager;


        #region Events

        internal readonly Event settingConfirmed = new Event();
        internal readonly Event creditsConfirmed = new Event();
        internal readonly Event gameStarted = new Event();
        internal readonly Event gameContinued = new Event();
        internal readonly Event exitStarted = new Event();
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
            characterImageComponent.RandomizeCharacterSprite();
            gameTitleComponent.UpdateTitle(characterImageComponent.GetCharacter());

            #region Listeners

            // TODO: These can be moved to the buttons.
            gameStarted.AddListener(() =>
            {
                commandManager.ExecuteCommand(new PlayGameCommand());
            });
            
            gameContinued.AddListener(() =>
            {
                commandManager.ExecuteCommand(new ContinueGameCommand());
            });

            settingConfirmed.AddListener(() =>
            {
                Instantiate(settingsMenuPrefab, transform.parent);
            });

            creditsConfirmed.AddListener(() =>
            {
                Instantiate(creditsDialoguePrefab, transform.parent);
            });
            
            exitStarted.AddListener(() =>
            {
                Instantiate(exitConfirmationPrefab, transform.parent);
            });

            #endregion
        }
    }
}
