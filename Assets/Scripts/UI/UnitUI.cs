using System;
using System.Linq;
using Commands;
using Managers;
using StatusEffects;
using TMPro;
using Units;
using Units.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitUI : MonoBehaviour
    {
        private IUnit selectedUnit;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI movementPointsText;
        [SerializeField] private TextMeshProUGUI attack;
        [SerializeField] private TextMeshProUGUI defence;
        [SerializeField] private TextMeshProUGUI speed;
        [SerializeField] private TextMeshProUGUI tenetUI;

        private void Start()
        {
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Pride, 3);
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Passion, 3);
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Sorrow, 3);
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Humility, 3);
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Joy, 3);
            // selectedPlayerUnit.AddOrReplaceTenetStatusEffect(TenetType.Apathy, 3);
            
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();
            
            commandManager.ListenCommand<UnitSelectedCommand>(cmd => SelectUnit());
            commandManager.ListenCommand<AbilityCommand>(cmd => SelectUnit());
            commandManager.ListenCommand<UnitDeselectedCommand>(cmd => DeselectUnit());
        }
        
        public void SelectUnit()
        {
            selectedUnit = ManagerLocator.Get<UnitManager>().SelectedUnit;
            
            if (selectedUnit != null)
                UpdateUnitUI();
            
            gameObject.SetActive(selectedUnit != null);
        }

        public void DeselectUnit() => gameObject.SetActive(false);
        
        public void UpdateUnitUI()
        {
            // TODO: Icon setup for realsies
            //icon.sprite = selectedPlayerUnit.sprite;
            
            // TODO: change to the unit's name if any
            nameText.text = selectedUnit.gameObject.name;
            health.text = "Health: " + selectedUnit.Health.HealthPoints.Value;
            movementPointsText.text = "MP: " + selectedUnit.MovementActionPoints.Value;
            attack.text = "Attack: " + selectedUnit.DealDamageModifier.Value;
            defence.text = "Defence: " + selectedUnit.Health.TakeDamageModifier.Value;
            speed.text = "Speed: " + selectedUnit.Speed.Value;
            
            string tenetText = String.Join("\n",selectedUnit.TenetStatusEffects.Select(t => t.TenetType+": " + t.StackCount) );
            tenetUI.text = tenetText;
        }
    }
}
