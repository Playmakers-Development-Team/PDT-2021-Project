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

        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}


        protected override void OnDialogueAwake()
        { 
            base.OnDialogueAwake();
            mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
            characterImageComponent.RandomizeCharacterSprite();
            gameTitleComponent.UpdateTitle(characterImageComponent.GetCharacter());
            
            exitStarted.AddListener(() =>
            {
                exitConfirmedPrefabInstance = Instantiate(exitConfirmationPrefab, mainMenuParent);
                Destroy(mainMenuPrefabInstance);
            });

            exitConfirmed.AddListener(() =>
            {
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false;
            });
            
            cancelExit.AddListener(() =>
            {
               mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
               Destroy(exitConfirmedPrefabInstance);
            });
            
            gameStarted.AddListener(() =>
            {
                //TODO: CHANGE TO APPROPRIATE SCENE LOADER OR SCENE
                SceneManager.LoadScene("Scenes/Design/Playtest Beta Map");
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
        }
        
        


    }
}
