using System.Threading.Tasks;
using TMPro;
using UI.Core;
using Units.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Unit
{
    public class UnitDisplay : DialogueComponent<GameDialogue>
    {
        [Header("Damage Text")]
        
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;
        
        [Header("Health Bar")]
        
        [SerializeField] private Image healthBarCurrent;
        [SerializeField] private Image healthBarDifference;
        [SerializeField] private float differenceDuration = 5;
        [SerializeField] private float differenceDelay = 1;

        private GameDialogue.UnitInfo unitInfo;

        private RectTransform rectTransform;
        private bool moving;
        
        
        #region MonoBehaviour

        private void LateUpdate()
        {
            if (!moving)
                return;
            
            rectTransform.position = unitInfo.Unit.transform.position;
        }

        #endregion
        
        
        #region UIComponent

        protected override void OnComponentAwake()
        {
            TryGetComponent(out rectTransform);
        }

        protected override void Subscribe()
        {
            dialogue.startedMove.AddListener(OnStartedMove);
            dialogue.endedMove.AddListener(OnEndedMove);
            dialogue.unitDamaged.AddListener(OnTakeDamage);
            dialogue.unitKilled.AddListener(OnUnitKilled);
        }

        protected override void Unsubscribe()
        {
            dialogue.startedMove.RemoveListener(OnStartedMove);
            dialogue.endedMove.RemoveListener(OnEndedMove);
            dialogue.unitDamaged.RemoveListener(OnTakeDamage);
            dialogue.unitKilled.RemoveListener(OnUnitKilled);
        }

        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            dialogue.unitSelected.Invoke(unitInfo);
        }

        private void OnStartedMove(GameDialogue.MoveInfo info)
        {
            if (info.UnitInfo.Unit != unitInfo.Unit)
                return;

            moving = true;
        }

        private void OnEndedMove(GameDialogue.UnitInfo info)
        {
            if (info.Unit != unitInfo.Unit)
                return;

            moving = false;
        }
        
        private void OnTakeDamage(GameDialogue.StatChangeInfo info)
        {
            if (info.Unit != unitInfo.Unit || info.StatType != StatTypes.Health)
                return;
            
            UpdateDamageText(info);
            UpdateHealthBar(info);
        }

        private void OnUnitKilled(GameDialogue.UnitInfo info)
        {
            if (info != unitInfo)
                return;

            Destroy(gameObject);
        }
        
        #endregion
        
        
        #region Drawing

        internal void Assign(GameDialogue.UnitInfo info)
        {
            unitInfo = info;
            rectTransform.position = unitInfo.Unit.transform.position;
        }

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
