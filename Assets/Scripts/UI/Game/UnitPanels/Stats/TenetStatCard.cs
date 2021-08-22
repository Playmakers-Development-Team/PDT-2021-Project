using TMPro;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Stats
{
    public class TenetStatCard : DialogueComponent<GameDialogue>
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image tenetImage;
        
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Drawing
        
        internal void Apply(Sprite icon, int value)
        {
            valueText.text = value.ToString();
            tenetImage.sprite = icon;
        }

        internal void Show()
        {
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }
        
        #endregion
    }
}
