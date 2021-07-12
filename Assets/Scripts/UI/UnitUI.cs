﻿using System;
using Cysharp.Threading.Tasks;
using GridObjects;
using Managers;
using TMPro;
using Units;
using Units.Commands;
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
            
            ManagerLocator.Get<CommandManager>().ListenCommand((TakeTotalDamageCommand cmd) => OnTakeDamage(cmd));
        }


        public void OnClick() => manager.unitSelected.Invoke(unit);

        private async void OnTakeDamage(TakeTotalDamageCommand cmd)
        {
            if (cmd.Unit != unit)
                return;
            
            await DamageTextDisplay(cmd.Value);
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
