using Audio;
using Game;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Event = UI.Core.Event;
using UI.Core;

namespace UI.PauseScreen
{
    public class PauseScreenDialogue : Dialogue
    {
        private GameManager gameManager;
        
        internal readonly Event buttonSelected = new Event();
        internal readonly Event continueGame = new Event();
        internal readonly Event settingsOpened = new Event();
        internal readonly Event exitGame = new Event();
        internal readonly Event restartEncounter = new Event();

        [SerializeField] private GameObject exitQueryDialogue;
        [SerializeField] private GameObject settingsMenuDialogue;

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();

            gameManager = ManagerLocator.Get<GameManager>();

            // TODO: Move to buttons.
            continueGame.AddListener(Resume);

            exitGame.AddListener(() =>
            {
                Instantiate(exitQueryDialogue, transform.parent);
            });
            
            settingsOpened.AddListener(() =>
            {
                Instantiate(settingsMenuDialogue, transform.parent);
            });
        }

        #region UIComponent

        protected override void OnClose() {}

        protected override void OnPromote() {}

        protected override void OnDemote() {}

        #endregion

        public void Resume()
        {
            ManagerLocator.Get<AudioManager>().ChangeMusicState("CombatState", "In_Combat");
            
            manager.Pop();
            
            gameManager.Resume();
        }
    }
}
