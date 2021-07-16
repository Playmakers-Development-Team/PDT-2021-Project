using System.Threading.Tasks;
using Grid.GridObjects;
using TMPro;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Unit
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


        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);

            unitInfo.SetUnit(unit);
        }

        protected override void OnComponentDisabled()
        {
            dialogue.unitDamaged.RemoveListener(OnTakeDamage);
        }

        protected override void Subscribe()
        {
            dialogue.unitSpawned.Invoke(unitInfo);
            
            dialogue.unitDamaged.AddListener(OnTakeDamage);
        }

        protected override void Unsubscribe() {}

        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            dialogue.unitSelected.Invoke(unitInfo);
        }
        
        private void OnTakeDamage(GameDialogue.StatChangeInfo data)
        {
            if (data.Unit != unit)
                return;
            
            UpdateDamageText(data);
            UpdateHealthBar(data);
        }
        
        #endregion
        
        
        #region Drawing

        private async void UpdateDamageText(GameDialogue.StatChangeInfo data)
        {
            damageText.text = data.Difference.ToString();
            damageText.enabled = true;
            
            await Task.Delay((int) damageTextDuration * 1000);

            // BUG: See below...
            if (damageText == null)
                return;
            
            damageText.enabled = false;
        }

        private async void UpdateHealthBar(GameDialogue.StatChangeInfo data)
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
                
                // BUG: Null reference here, preventing with scuffed check...
                // I think the IUnit can die while this async function runs, it should kill the function
                //  by listening to dialogue.unitKilled but I don't know how best to do that right now.
                // BUG: ALSO need to abort these if the unit is damaged again, or it'll wait for this to start...
                
                if (healthBarDifference == null)
                    return;
                    
                healthBarDifference.fillAmount = Mathf.Lerp(tOld, tCurrent, t);
                await Task.Yield();
            }

            healthBarDifference.fillAmount = 0.0f;
        }
        
        #endregion
    }
}
