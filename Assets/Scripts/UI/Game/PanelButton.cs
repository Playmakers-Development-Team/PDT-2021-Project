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
        dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
    }

    protected override void Unsubscribe()
    {
        dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
    }

    protected override void OnComponentAwake()
    {
        button.onClick.AddListener(OnClick);
    }

    #endregion
    
    
    #region Listeners
    
    private void OnClick()
    {
        if (!clicked)
            Selected();
        else
            Deselected();
    }

    private void Selected()
    {
        OnSelected();
     
        clicked = true;
        EventSystem.current.SetSelectedGameObject(button.gameObject);
        labelText.font = selectedFont;
    }

    private void Deselected()
    {
        clicked = false;
        EventSystem.current.SetSelectedGameObject(null);
        labelText.font = defaultFont;
        
        OnDeselected();
    }

    private void OnAbilityDeselected(Ability ability)
    {
        Deselected();
    }

    #endregion

    
    #region PanelButton

    protected virtual void OnSelected()
    {
        dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
    }

    protected virtual void OnDeselected() {}
    
    #endregion
}