using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI.Refactored
{
    public class ValueStatCard : Element
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        public void Apply(string label, int value)
        {
            labelText.text = label;
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
