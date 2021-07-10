using System;
using Cysharp.Threading.Tasks;
using GridObjects;
using TMPro;
using Units;
using UnityEngine;

namespace UI
{
    public class UnitUI : Element
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;

        private float lastHealth;
        private IUnit unit;


        protected override void OnAwake()
        {
            
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);
            
            // TODO: Listen to unit.onTakeDamage!
        }

        private void LateUpdate()
        {
            float health = unit.Health.HealthPoints.Value;
            if (health < lastHealth)
                OnTakeDamage((int) (lastHealth - health));
            lastHealth = health;
        }


        public void OnClick() => manager.unitSelected.Invoke(unit);

        private async void OnTakeDamage(int damage)
        {
            await DamageTextDisplay(damage);
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
