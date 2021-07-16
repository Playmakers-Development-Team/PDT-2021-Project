using System.Globalization;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Game.UnitPanels.Stats
{
    public class ValueStatCard : UIComponent<GameDialogue>
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Drawing
        
        public void Apply(string label, int value)
        {
            labelText.text = label;
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
        
        #endregion
    }
}
