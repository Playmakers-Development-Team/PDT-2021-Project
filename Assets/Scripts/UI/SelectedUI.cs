using TMPro;
using System.Collections.Generic;
using Units;
using StatusEffects;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectedUI : MonoBehaviour
    {
        [SerializeField] private Image unitImage;
        //[SerializeField] private TextMeshProUGUI unitName;
        private List<TextMeshProUGUI> abilityUIText;
        [SerializeField] private GameObject abilityUIPrefab;
        
        public IUnit Unit { get; private set; }
    }
}
