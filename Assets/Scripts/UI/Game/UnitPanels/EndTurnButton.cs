using UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Game.UnitPanels
{
    public class EndTurnButton : DialogueComponent<GameDialogue>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private EventTrigger eventTrigger;
        
        private static readonly int normalId = Animator.StringToHash("Normal");
        private static readonly int highlightedId = Animator.StringToHash("Highlighted");


        protected override void OnComponentAwake()
        {
            EventTrigger.Entry enter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            enter.callback.AddListener(data => animator.SetTrigger(highlightedId));
            eventTrigger.triggers.Add(enter);

            EventTrigger.Entry exit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            exit.callback.AddListener(data => animator.SetTrigger(normalId));
            eventTrigger.triggers.Add(exit);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
