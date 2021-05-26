using System.Collections;
using System.Collections.Generic;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitCard : MonoBehaviour
    {
        [SerializeField] private Image unitImage;
        [SerializeField] private TextMeshProUGUI unitName;
        
        public void SetCardImageAs(IUnit unit)
        {
            if (unit is EnemyUnit)
                unitImage.color = Color.black;
            
        }

        public void SetUnitText(string unitName)
        {
            this.unitName.text = unitName;
        }

    }
}
