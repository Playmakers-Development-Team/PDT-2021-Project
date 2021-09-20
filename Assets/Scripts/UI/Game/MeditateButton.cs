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
            turnManager.Meditate();
            dialogue.meditateConfirmed.Invoke(dialogue.SelectedUnit);

            icon.sprite = activatedSprite;

            
            dialogue.buttonSelected.Invoke();
            
            SetInteractable(false);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            SetInteractable(info.IsPlayer);
            
            if (!info.IsPlayer || turnManager.ActingUnit == null)
                return;

            bool unitCanMeditate = turnManager.UnitCanBeTurnManipulated(turnManager.ActingUnit);
            
            interactable = unitCanMeditate;
            icon.sprite = unitCanMeditate ? defaultSprite : activatedSprite;
            
            SetInteractable(unitCanMeditate);
        }
    }
}
