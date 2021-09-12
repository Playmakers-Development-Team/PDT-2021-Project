using System.Linq;
using System.Threading.Tasks;
using TenetStatuses;
using TMPro;
using UI.Core;
using Units.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Unit
{
    public class UnitDisplay : DialogueComponent<GameDialogue>
    {
        [Header("Button")]
        
        [SerializeField] private Button button;
        
        [Header("Damage Text")]
        
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;
        
        [Header("Health Bar")]
        
        [SerializeField] private Image healthBarCurrent;
        [SerializeField] private Image healthBarDifference;
        [SerializeField] private float differenceDuration = 5;
        [SerializeField] private float differenceDelay = 1;

        [Header("Stat Cards")]
        
        [SerializeField] private RectTransform statArea;

        [SerializeField] private StatDisplay attackDisplay;
        [SerializeField] private StatDisplay defenceDisplay;
        [SerializeField] private TenetDisplay primaryTenetDisplay;
        [SerializeField] private TenetDisplay secondaryTenetDisplay;


        private GameDialogue.UnitInfo unitInfo;

        private RectTransform rectTransform;
        private bool moving;
        private static readonly int fillId = Shader.PropertyToID("_Fill");


        internal GameDialogue.UnitInfo UnitInfo => unitInfo;
        
        
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

        protected override void OnComponentStart()
        {
            healthBarCurrent.material = Instantiate(healthBarCurrent.material);

            UpdateStatDisplays();
        }

        protected override void Subscribe()
        {
            dialogue.startedMove.AddListener(OnStartedMove);
            dialogue.endedMove.AddListener(OnEndedMove);
            dialogue.unitStatChanged.AddListener(OnUnitStatChanged);
            dialogue.unitKilled.AddListener(OnUnitKilled);
            dialogue.turnStarted.AddListener(OnTurnStarted);
        }

        protected override void Unsubscribe()
        {
            dialogue.startedMove.RemoveListener(OnStartedMove);
            dialogue.endedMove.RemoveListener(OnEndedMove);
            dialogue.unitStatChanged.RemoveListener(OnUnitStatChanged);
            dialogue.unitKilled.RemoveListener(OnUnitKilled);
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Default)
                return;
            
            dialogue.unitSelected.Invoke(unitInfo);
        }

        private void OnStartedMove(GameDialogue.MoveInfo info)
        {
            button.enabled = false;
            
            if (info.UnitInfo.Unit != unitInfo.Unit)
                return;

            moving = true;
        }

        private void OnEndedMove(GameDialogue.UnitInfo info)
        {
            button.enabled = true;
            
            if (info.Unit != unitInfo.Unit)
                return;

            moving = false;
        }
        
        private void OnUnitStatChanged(GameDialogue.StatChangeInfo info)
        {
            if (info.Unit != unitInfo.Unit)
                return;
            
            switch (info.StatType)
            {
                case StatTypes.Health:
                    UpdateDamageText(info);
                    UpdateHealthBar(info);
                    break;
                
                case StatTypes.Attack:
                    // UpdateStatDisplays();
                    break;
                
                case StatTypes.Defence:
                    // UpdateStatDisplays();
                    break;
            }
        }

        private void OnUnitKilled(GameDialogue.UnitInfo info)
        {
            if (info != unitInfo)
                return;

            Destroy(gameObject);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            UpdateStatDisplays();
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
            damageText.text = data.DisplayValue.ToString();
            damageText.enabled = true;
            
            await Task.Delay((int) damageTextDuration * 1000);

            if (damageText == null)
                return;
            
            damageText.enabled = false;
        }

        private async void UpdateHealthBar(GameDialogue.StatChangeInfo data)
        {
            if (data.Difference == 0)
                return;
            
            float baseAmount = data.BaseValue;
            
            float oldAmount = data.OldValue;
            float currentAmount = data.NewValue;
            
            float tOld = oldAmount / baseAmount;
            float tCurrent = currentAmount / baseAmount;

            healthBarCurrent.material.SetFloat(fillId, tCurrent);
            // healthBarDifference.material.SetFloat(fillId, tOld);
            
            await Task.Delay((int) (differenceDelay * 1000));

            float start = Time.time;
            float duration = (tOld - tCurrent) * differenceDuration;

            while (Time.time - start < duration)
            {
                float t = (Time.time - start) / duration;
                
                if (healthBarDifference == null)
                    return;
                    
                // healthBarDifference.fillAmount = Mathf.Lerp(tOld, tCurrent, t);
                await Task.Yield();
            }

            // healthBarDifference.fillAmount = 0.0f;
        }

        private void UpdateStatDisplays()
        {
            if (unitInfo.Unit.AttackStat.Value == 0)
            {
                attackDisplay.gameObject.SetActive(false);
            }
            else
            {
                attackDisplay.gameObject.SetActive(true);
                attackDisplay.Assign(unitInfo.Unit.AttackStat.Value);
            }
            
            if (unitInfo.Unit.DefenceStat.Value == 0)
            {
                defenceDisplay.gameObject.SetActive(false);
            }
            else
            {
                defenceDisplay.gameObject.SetActive(true);
                defenceDisplay.Assign(unitInfo.Unit.DefenceStat.Value);
            }

            TenetStatus[] statuses = unitInfo.Unit.TenetStatuses.ToArray();

            if (statuses.Length > 0 && !statuses[0].IsEmpty)
            {
                primaryTenetDisplay.gameObject.SetActive(true);
                primaryTenetDisplay.Assign(statuses[0].StackCount);
                primaryTenetDisplay.SetTenet(statuses[0].TenetType);
            }
            else
            {
                primaryTenetDisplay.gameObject.SetActive(false);
            }

            if (statuses.Length > 1 && !statuses[1].IsEmpty)
            {
                secondaryTenetDisplay.gameObject.SetActive(true);
                secondaryTenetDisplay.Assign(statuses[1].StackCount);
                secondaryTenetDisplay.SetTenet(statuses[1].TenetType);
            }
            else
            {
                secondaryTenetDisplay.gameObject.SetActive(false);
            }
            
            RepositionStatDisplays();
        }
        
        private void RepositionStatDisplays()
        {
            StatDisplay[] displays =
                {attackDisplay, defenceDisplay, primaryTenetDisplay, secondaryTenetDisplay};

            StatDisplay[] activeDisplays =
                displays.Where(d => d.gameObject.activeInHierarchy).ToArray();
            
            Vector3[] corners = new Vector3[4];
            statArea.GetWorldCorners(corners);
            
            Vector3 start =
                Vector3.Lerp(corners[1], corners[3], 1.0f / (activeDisplays.Length + 1.0f));
            
            Vector3 end =
                Vector3.Lerp(corners[3], corners[1], 1.0f / (activeDisplays.Length + 1.0f));
            
            for (int i = 0; i < activeDisplays.Length; i++)
            {
                float t = activeDisplays.Length == 1 ? 0.0f : i / (activeDisplays.Length - 1.0f);
                Vector3 target = Vector3.Lerp(start, end, t);

                activeDisplays[i].transform.position = target;
            }
        }
        
        #endregion
    }
}
