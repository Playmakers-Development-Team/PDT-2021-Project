using System.Globalization;
using TMPro;
using TenetStatuses;
using UnityEngine;

namespace UI
{
    public class TenetStatCard : Element
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        
        
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
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
