using Game;
using Managers;
using UnityEngine;

namespace UI.Game
{
    public class PauseButton : PanelButton
    {
        [SerializeField] private GameObject pauseMenuDialoguePrefab;

        private GameManager gameManager;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();

            gameManager = ManagerLocator.Get<GameManager>();
        }

        protected override void OnSelected()
        {
            Instantiate(pauseMenuDialoguePrefab, dialogue.transform.parent);
            
            gameManager.Pause();
        }
    }
}
