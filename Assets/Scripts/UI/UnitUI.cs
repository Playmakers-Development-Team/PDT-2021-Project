using System;
using Commands;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using Managers;
using TMPro;
using Units;
using Units.Commands;
using Units.Stats;
using UnityEngine;

namespace UI
{
    public class UnitUI : Element
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;

        private IUnit unit;


        protected override void OnAwake()
        {
            
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);
            
            ManagerLocator.Get<CommandManager>().ListenCommand((StatChangedCommand cmd) => OnTakeDamage(cmd));
        }


        public void OnClick() => manager.unitSelected.Invoke(unit);

        private async void OnTakeDamage(StatChangedCommand cmd)
        {
            if (cmd.Unit != unit || cmd.StatType != StatTypes.Health)
                return;
            
            await DamageTextDisplay(cmd.DisplayValue);
            DamageTextHide();
        }

        private UniTask DamageTextDisplay(int damage)
        {
            damageText.text = damage.ToString();
            damageText.enabled = true;
            return UniTask.Delay((int) damageTextDuration * 1000);
        }

        private void DamageTextHide()
        {
            damageText.enabled = false;
        }
    }
}
