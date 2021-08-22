using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Game.UnitPanels
{
    public class EndTurnButton : DialogueComponent<GameDialogue>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private EventTrigger eventTrigger;
        
        private CommandManager commandManager;
        private TurnManager turnManager;
        
        private static readonly int normalId = Animator.StringToHash("Normal");
        private static readonly int highlightedId = Animator.StringToHash("Highlighted");


        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            
            EventTrigger.Entry enter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            enter.callback.AddListener(data => animator.SetTrigger(highlightedId));
            eventTrigger.triggers.Add(enter);

            EventTrigger.Entry exit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            exit.callback.AddListener(data => animator.SetTrigger(normalId));
            eventTrigger.triggers.Add(exit);

            EventTrigger.Entry pointerClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            pointerClick.callback.AddListener(data =>
            {
                if (turnManager.ActingPlayerUnit == null)
                    return;
                
                commandManager.ExecuteCommand(new EndTurnCommand(turnManager.ActingPlayerUnit));
            });
            eventTrigger.triggers.Add(pointerClick);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
