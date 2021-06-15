using TMPro;
using Unit;
using Unit.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitCard : MonoBehaviour
    {
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitName;
        
        public IUnit Unit { get; private set; }

        public void SetUnit(IUnit unit)
        {
            Unit = unit;
            
            SetCardImageAs(unit);
            SetUnitText(unit.ToString());
        }

        private void SetCardImageAs(IUnit unit)
        {
            if (unit is EnemyUnit)
                unitImage.color = Color.black;
        }

        private void SetUnitText(string unitName)
        {
            this.unitName.text = unitName;
        }
    }
}
