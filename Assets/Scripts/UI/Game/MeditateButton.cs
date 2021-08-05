using Managers;
using Turn;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class MeditateButton : PanelButton
    {
        [Header("Meditate Button")]
        
        [SerializeField] private Image icon;

        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite activatedSprite;

        private TurnManager turnManager;


        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            dialogue.turnStarted.AddListener(OnTurnStarted);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        protected override void OnSelected()
        {
            dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
            dialogue.meditateConfirmed.Invoke(dialogue.SelectedUnit);

            icon.sprite = activatedSprite;

            turnManager.Meditate();
            dialogue.buttonSelected.Invoke();
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            // TODO: Add proper boolean once implemented...
            bool unitIsMeditating = false;
            icon.sprite = unitIsMeditating ? activatedSprite : defaultSprite;
        }
    }
}
