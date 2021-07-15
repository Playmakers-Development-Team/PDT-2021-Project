using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI.Game.UnitPanels
{
    public class ValueStatCard : UIComponent<GameDialogue>
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        public void Apply(string label, int value)
        {
            labelText.text = label;
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
