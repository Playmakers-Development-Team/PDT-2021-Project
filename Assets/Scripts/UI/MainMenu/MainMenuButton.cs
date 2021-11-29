using TMPro;
using UI.Core;
using UI.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.MainMenu
{
    // TODO: Duplicate code. See PauseScreenButton.
    public class MainMenuButton : DialogueComponent<MainMenuDialogue>
    {
        [SerializeField] protected EventTrigger trigger;
        [SerializeField] protected Animator animator;
        [SerializeField] protected TextMeshProUGUI labelText;

        [SerializeField] protected Image backgroundImage;
        [SerializeField] protected Image borderImage;

        [Header("Sprites")] [SerializeField]
        protected Sprite backgroundLight;

        [SerializeField] protected Sprite backgroundDark;

        [SerializeField] protected Sprite borderLight;
        [SerializeField] protected Sprite borderDark;

        [Header("Fonts")] [SerializeField]
        private TMP_FontAsset lightFont;

        [SerializeField] private TMP_FontAsset darkFont;

        [SerializeField, Range(0f, 1f)]
        private float fill;

        [SerializeField] protected bool interactable;

        private bool clicked;
        private bool animating;
        private bool wasInteractable;

        private static readonly int borderFillId = Animator.StringToHash("Fill");
        private static readonly int borderFadeId = Animator.StringToHash("Fade");
        private static readonly int fillId = Shader.PropertyToID("_Fill");

        private void LateUpdate()
        {
            if (!animating)
                return;

            borderImage.material.SetFloat(fillId, fill);
        }

        public void AnimationStarted()
        {
            animating = true;
        }

        public void FillComplete()
        {
            animating = false;
            borderImage.material.SetFloat(fillId, 1.0f);
        }

        public void FadeComplete()
        {
            animating = false;
            borderImage.material.SetFloat(fillId, 0.0f);
        }

        protected void SetInteractable(bool state)
        {
            if (interactable)
            {
                wasInteractable = state;
                interactable = state;
            }
            else
            {
                wasInteractable = state;
                interactable = state;
            }
        }

        #region DialogueComponent

        protected override void Subscribe()
        {
            dialogue.buttonSelected.AddListener(OnButtonSelected);
            dialogue.promoted.AddListener(OnPromoted);
            dialogue.demoted.AddListener(OnDemoted);
        }

        protected override void Unsubscribe()
        {
            dialogue.buttonSelected.AddListener(OnButtonSelected);
            dialogue.promoted.RemoveListener(OnPromoted);
            dialogue.demoted.RemoveListener(OnDemoted);
        }

        protected override void OnComponentAwake()
        {
            wasInteractable = interactable;

            backgroundImage.material = Instantiate(backgroundImage.material);
            borderImage.material = Instantiate(borderImage.material);

            EventTrigger.Entry pointerEnter =
                new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            pointerEnter.callback.AddListener(info =>
            {
                OnHoverEnter();

                if (clicked || !interactable)
                    return;

                backgroundImage.sprite = backgroundLight;

                borderImage.sprite = borderDark;
                animator.SetTrigger(borderFillId);

                labelText.font = darkFont;
            });
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit =
                new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            pointerExit.callback.AddListener(info =>
            {
                OnHoverExit();

                if (clicked || !interactable)
                    return;

                backgroundImage.sprite = backgroundLight;

                animator.SetTrigger(borderFadeId);

                labelText.font = darkFont;
            });
            trigger.triggers.Add(pointerExit);

            EventTrigger.Entry pointerClick =
                new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            pointerClick.callback.AddListener(info =>
            {
                if (!interactable)
                    return;

                if (!clicked)
                    Selected();
                else
                    Deselected();
            });
            trigger.triggers.Add(pointerClick);

            EventTrigger.Entry pointerDown =
                new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};
            pointerDown.callback.AddListener(info =>
            {
                if (!interactable)
                    return;

                backgroundImage.sprite = backgroundDark;
                borderImage.sprite = borderLight;
                labelText.font = lightFont;
            });
            trigger.triggers.Add(pointerDown);
        }

        #endregion

        #region Listeners

        private void OnButtonSelected()
        {
            if (!clicked)
                return;

            Deselected();
            animator.SetTrigger(borderFadeId);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            if (!clicked)
                return;

            Deselected();
            animator.SetTrigger(borderFadeId);
        }

        private void Selected()
        {
            clicked = true;
            EventSystem.current.SetSelectedGameObject(gameObject);

            OnSelected();
            
            Deselected();
        }

        private void Deselected()
        {
            clicked = false;
            EventSystem.current.SetSelectedGameObject(null);

            backgroundImage.sprite = backgroundLight;
            borderImage.sprite = borderDark;
            labelText.font = darkFont;

            OnDeselected();
        }

        private void OnDemoted()
        {
            wasInteractable = interactable;
            interactable = false;
        }

        private void OnPromoted()
        {
            interactable = wasInteractable;
        }

        #endregion

        #region PanelButton

        protected virtual void OnSelected() {}

        protected virtual void OnDeselected() {}

        protected virtual void OnHoverEnter() {}

        protected virtual void OnHoverExit() {}

        #endregion
    }
}
