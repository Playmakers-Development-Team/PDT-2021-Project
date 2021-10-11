using UI.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Event = UI.Core.Event;

namespace UI.MainMenu
{
    public class MainMenuDialogue : Dialogue
    {

        [SerializeField] private GameObject gameTitle;
        [SerializeField] private CharacterImageComponent characterImageComponent;
        [SerializeField] private Transform mainMenuParent;
        [SerializeField] private GameObject exitConfirmationPrefab;
        [SerializeField] private GameObject MainMenuButtonPrefab;
        [SerializeField] private GameObject splashScreenPrefab;

        private GameObject exitConfirmedPrefabInstance;
        private GameObject mainMenuPrefabInstance;
        private GameObject splashScreenInstance;

        internal bool isOnSplashScreen;
        internal bool hasSpawnedMainMenuOnce;
        
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
        internal readonly Event splashScreenEnded = new Event();
        internal readonly Event splashScreenStart = new Event();
        internal readonly Event startTitleAnimation = new Event();
        
        #endregion

        #region DialogueComponent
        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        #endregion

        protected override void OnDialogueAwake()
        { 
            base.OnDialogueAwake();
            isOnSplashScreen = true;
            splashScreenInstance = Instantiate(splashScreenPrefab, mainMenuParent);
            hasSpawnedMainMenuOnce = false;
           // mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
        //    characterImageComponent.RandomizeCharacterSprite();
//            gameTitleComponent.UpdateTitle(characterImageComponent.GetCharacter());
            Instantiate(gameTitle, mainMenuParent);
            #region Listeners

            exitStarted.AddListener(() =>
            {
                exitConfirmedPrefabInstance = Instantiate(exitConfirmationPrefab, mainMenuParent);
                Destroy(mainMenuPrefabInstance);
            });

            exitConfirmed.AddListener(() =>
            {
                
                Application.Quit();
               // UnityEditor.EditorApplication.isPlaying = false;
            });
            
            cancelExit.AddListener(() =>
            {
               mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
               Destroy(exitConfirmedPrefabInstance);
               
               if (hasSpawnedMainMenuOnce)
                   mainMenuPrefabInstance.GetComponent<Animator>().SetTrigger("Normal");
               
            });
            
            gameStarted.AddListener(() =>
            {
                //TODO: CHANGE TO APPROPRIATE SCENE LOADER OR SCENE
                SceneManager.LoadScene("Scenes/Design/Playtest Beta Map");
            });
            
            splashScreenEnded.AddListener(() =>
            {
                mainMenuPrefabInstance = Instantiate(MainMenuButtonPrefab, mainMenuParent);
                Destroy(splashScreenInstance);
                isOnSplashScreen = false;

                if (hasSpawnedMainMenuOnce)
                {
                    mainMenuPrefabInstance.GetComponent<Animator>().SetTrigger("Normal");
                    return;
                }
                
                hasSpawnedMainMenuOnce = true;
            });
            
            splashScreenStart.AddListener(() =>
            {
                splashScreenInstance = Instantiate(splashScreenPrefab, mainMenuParent);
                Destroy(mainMenuPrefabInstance);
                isOnSplashScreen = true;
                
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
