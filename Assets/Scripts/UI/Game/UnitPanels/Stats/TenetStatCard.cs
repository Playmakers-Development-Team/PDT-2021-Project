using TenetStatuses;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Game.UnitPanels.Stats
{
    public class TenetStatCard : DialogueComponent<GameDialogue>
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Drawing
        
        public void Apply(TenetType tenet, int value)
        {
            string label = tenet switch
            {
                TenetType.Pride => "PR",
                TenetType.Humility => "H",
                TenetType.Passion => "PS",
                TenetType.Apathy => "A",
                TenetType.Joy => "J",
                TenetType.Sorrow => "S",
                _ => "ERR"
            };

            labelText.text = label;
            valueText.text = value.ToString();
        }
        
        #endregion
    }
}
