using Abilities;
using TMPro;
using UI.Core;
using UI.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelButton : DialogueComponent<GameDialogue>
{
    [SerializeField] protected Button button;
    [SerializeField] protected TextMeshProUGUI labelText;
        
    [Header("Fonts")]
        
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset selectedFont;
    
    private bool clicked;

    
    #region DialogueComponent

    protected override void Subscribe()
    {
        dialogue.buttonSelected.AddListener(OnButtonSelected);
        dialogue.turnStarted.AddListener(OnTurnStarted);
    }

    protected override void Unsubscribe()
    {
        dialogue.buttonSelected.RemoveListener(OnButtonSelected);
        dialogue.turnStarted.RemoveListener(OnTurnStarted);
    }

    protected override void OnComponentAwake() => button.onClick.AddListener(OnClick);

    #endregion
    
    
    #region Listeners

    private void OnButtonSelected() => TryDeselect();

    private void OnTurnStarted(GameDialogue.TurnInfo info) => TryDeselect();
    
    private void OnClick()
    {
        if (!clicked)
            Selected();
        else
            TryDeselect();
    }

    private void TryDeselect()
    {
        if (clicked)
            Deselected();
    }

    private void Selected()
    {
        dialogue.buttonSelected.Invoke();
        
        clicked = true;
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        labelText.font = selectedFont;
        
        OnSelected();
    }

    private void Deselected()
    {
        clicked = false;
        EventSystem.current.SetSelectedGameObject(null);
        labelText.font = defaultFont;
        
        OnDeselected();
    }

    #endregion

    
    #region PanelButton

    protected virtual void OnSelected() {}

    protected virtual void OnDeselected() {}
    
    #endregion
}