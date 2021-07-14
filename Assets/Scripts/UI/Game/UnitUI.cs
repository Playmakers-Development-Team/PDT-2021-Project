using System.Threading.Tasks;
using Grid.GridObjects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitUI : UIComponent<GameDialogue>
    {
        [SerializeField] private GameDialogue.UnitInfo unitInfo;
        
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


        protected override void OnComponentAwake()
        {
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);

            unitInfo.SetUnit(unit);
        }

        protected override void Subscribe()
        {
            dialogue.unitSpawned.Invoke(unitInfo);
            
            dialogue.unitDamaged.AddListener(OnTakeDamage);
        }

        protected override void Unsubscribe() {}

        protected override void OnComponentDisabled()
        {
            dialogue.unitDamaged.RemoveListener(OnTakeDamage);
        }


        public void OnClick()
        {
            dialogue.unitSelected.Invoke(unitInfo);
        }

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
            if (data.Difference <= 0)
                return;
            
            float baseAmount = data.BaseValue;
            
            float oldAmount = data.OldValue;
            float currentAmount = data.NewValue;
            
            float tOld = oldAmount / baseAmount;
            float tCurrent = currentAmount / baseAmount;
            
            healthBarCurrent.fillAmount = tCurrent;
            healthBarDifference.fillAmount = tOld;

            await Task.Delay((int) (differenceDelay * 1000));

            float start = Time.time;
            float duration = (healthBarDifference.fillAmount - healthBarCurrent.fillAmount) * differenceDuration;

            while (Time.time - start < duration)
            {
                float t = (Time.time - start) / duration;
                healthBarDifference.fillAmount = Mathf.Lerp(tOld, tCurrent, t);
                await Task.Yield();
            }

            healthBarDifference.fillAmount = 0.0f;
        }
    }
}
