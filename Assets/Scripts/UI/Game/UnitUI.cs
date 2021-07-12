using System.Threading.Tasks;
using GridObjects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitUI : Element
    {
        [SerializeField] private GridObject unitGridObject;
        
        [Header("Damage Text")]
        
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;
        
        [Header("Health Bar")]
        
        [SerializeField] private Image healthBarCurrent;
        [SerializeField] private Image healthBarDifference;
        [SerializeField] private float differenceDuration = 5;
        [SerializeField] private float differenceDelay = 1;

        private IUnit unit;


        protected override void OnAwake()
        {
            
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);

            manager.unitDamaged.AddListener(OnTakeDamage);
        }

        private void OnDisable()
        {
            manager.unitDamaged.RemoveListener(OnTakeDamage);
        }


        public void OnClick() => manager.unitSelected.Invoke(unit);

        private void OnTakeDamage(StatDifference data)
        {
            if (data.Unit != unit)
                return;
            
            UpdateDamageText(data);
            UpdateHealthBar(data);
        }

        private async void UpdateDamageText(StatDifference data)
        {
            damageText.text = data.Difference.ToString();
            damageText.enabled = true;
            
            await Task.Delay((int) damageTextDuration * 1000);
            
            damageText.enabled = false;
        }

        private async void UpdateHealthBar(StatDifference data)
        {
            // TODO: Update to use data.OldValue to reduce number of calculations...
            if (data.Difference <= 0)
                return;
            
            float baseAmount = data.BaseValue;
            float currentAmount = data.NewValue;
            float tCurrent = currentAmount / baseAmount;
            float tDifference = data.Difference / baseAmount;
            
            healthBarCurrent.fillAmount = tCurrent;
            healthBarDifference.fillAmount = tCurrent + tDifference;

            await Task.Delay((int) (differenceDelay * 1000));

            float start = Time.time;
            float duration = (healthBarDifference.fillAmount - healthBarCurrent.fillAmount) * differenceDuration;

            while (Time.time - start < duration)
            {
                float t = (Time.time - start) / duration;
                healthBarDifference.fillAmount = Mathf.Lerp(tDifference + tCurrent, tCurrent, t);
                await Task.Yield();
            }

            healthBarDifference.fillAmount = 0.0f;
        }
    }
}
