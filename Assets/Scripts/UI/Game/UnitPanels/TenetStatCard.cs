using TenetStatuses;
using TMPro;
using UnityEngine;

namespace UI.Game.UnitPanels
{
    public class TenetStatCard : UIComponent<GameDialogue>
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
            valueText.text = value.ToString();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
