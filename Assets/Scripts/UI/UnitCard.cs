using System.Collections;
using System.Collections.Generic;
using Managers;
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
        [SerializeField] public IUnit Unit { get; private set; }



        public void SetUnit(IUnit unit)
        {
            Unit = unit;
        }
        
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
