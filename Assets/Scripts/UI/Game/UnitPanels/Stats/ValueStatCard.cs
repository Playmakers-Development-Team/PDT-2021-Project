using System.Globalization;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Game.UnitPanels.Stats
{
    public class ValueStatCard : DialogueComponent<GameDialogue>
    {
        [SerializeField] private TextMeshProUGUI valueText;
        

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Drawing
        
        public void Apply(int value)
        {
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
        
        #endregion
    }
}
