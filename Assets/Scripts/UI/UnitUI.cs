using System;
using System.Linq;
using StatusEffects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] private PlayerUnit selectedPlayerUnit;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI name;
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI attack;
        [SerializeField] private TextMeshProUGUI defence;
        [SerializeField] private TextMeshProUGUI tenetUI;

        private void Start()
        {
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Pride, 3);
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Passion, 3);
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Sorrow, 3);
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Humility, 3);
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Joy, 3);
            selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Apathy, 3);
            SelectUnit();
        }

        // TODO: Hook up with the real selected unit
        public void SelectUnit()
        {
            gameObject.SetActive(true);
            UpdateUnitUI();
        }
        
        public void UpdateUnitUI()
        {
            // TODO: Icon setup for realsies
            //icon.sprite = selectedPlayerUnit.sprite;
            
            // TODO: change to the unit's name if any
            name.text = selectedPlayerUnit.gameObject.name;
            health.text = "Health: " + selectedPlayerUnit.HealthPoints;
            attack.text = "Attack: " + selectedPlayerUnit.DealDamageModifier.Value;
            defence.text = "Defence: " + selectedPlayerUnit.TakeDamageModifier.Value;
            
            string tenetText = String.Join("\n",selectedPlayerUnit.TenetStatusEffects.Select(t => t.TenetType+": " + t.StackCount) );
            tenetUI.text = tenetText;
        }
    }
}
