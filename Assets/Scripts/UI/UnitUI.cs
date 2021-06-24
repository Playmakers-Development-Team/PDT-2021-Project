using System;
using System.Linq;
using Commands;
using Managers;
using StatusEffects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitUI : MonoBehaviour
    {
        private IUnit selectedPlayerUnit;
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
            commandManager.ListenCommand<UnitSelectedCommand>(cmd =>
            {
                SelectUnit();
            });
            commandManager.ListenCommand<AbilityCommand>(cmd =>
            {
                SelectUnit();
            });
            commandManager.ListenCommand<UnitDeselectedCommand>(cmd =>
            {
                DeselectUnit();
            });
        }

        // TODO: Hook up with the real selected unit
        public void SelectUnit()
        {
            selectedPlayerUnit = ManagerLocator.Get<PlayerManager>().SelectedUnit;
            if (selectedPlayerUnit != null)
            {
                gameObject.SetActive(true);
                UpdateUnitUI();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void DeselectUnit() => gameObject.SetActive(false);
        
        public void UpdateUnitUI()
        {
            // TODO: Icon setup for realsies
            //icon.sprite = selectedPlayerUnit.sprite;
            
            // TODO: change to the unit's name if any
            nameText.text = selectedPlayerUnit.gameObject.name;
            health.text = "Health: " + selectedPlayerUnit.Health.HealthPoints.Value;
            movementPointsText.text = "MP: " + selectedPlayerUnit.MovementActionPoints.Value;
            attack.text = "Attack: " + selectedPlayerUnit.DealDamageModifier.Value;
            defence.text = "Defence: " + selectedPlayerUnit.Health.TakeDamageModifier.Value;
            speed.text = "Speed: " + selectedPlayerUnit.Speed.Value;
            
            string tenetText = String.Join("\n",selectedPlayerUnit.TenetStatusEffects.Select(t => t.TenetType+": " + t.StackCount) );
            tenetUI.text = tenetText;
        }
    }
}
