using System.Linq;
using System.Threading.Tasks;
using Abilities;
using Commands;
using Managers;
using TenetStatuses;
using TMPro;
using Turn;
using UI.Core;
using Units;
using Units.Stats;
using Units.Virtual;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Game.Unit
{
    public class UnitDisplay : DialogueComponent<GameDialogue>
    {
        [Header("Button")]
        
        [SerializeField] private EventTrigger eventTrigger;
        
        [Header("Damage Text")]
        
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float damageTextDuration;
        
        [Header("Health Bar")]
        
        [SerializeField] private Image healthBarContainer;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Color hoverHealthColour;
        [SerializeField] private Color defaultHealthColour;
        
        [SerializeField] private Image healthBarCurrent;
        [SerializeField] private Image healthBarDifference;
        [SerializeField] private Image healthBarProjection;
        [SerializeField] private float differenceDuration = 5;
        [SerializeField] private float differenceDelay = 1;

        [Header("Stat Cards")]
        
        [SerializeField] private RectTransform statArea;

        [SerializeField] private StatDisplay attackDisplay;
        [SerializeField] private StatDisplay defenceDisplay;
        [SerializeField] private TenetDisplay primaryTenetDisplay;
        [SerializeField] private TenetDisplay secondaryTenetDisplay;

        [Header("Indicator")]
        
        [SerializeField] private Animator indicatorAnimator;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Color defaultIndicatorColour;
        [SerializeField] private Color selectedIndicatorColour;


        private GameDialogue.UnitInfo unitInfo;
        private TurnManager turnManager;
        private CommandManager commandManager;

        private RectTransform rectTransform;
        private bool moving;
        private static readonly int fillId = Shader.PropertyToID("_Fill");
        private static readonly int raisedId = Animator.StringToHash("raised");


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
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
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
            dialogue.unitSelected.AddListener(OnUnitSelected);
            dialogue.unitDeselected.AddListener(OnUnitDeselected);
            
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
            dialogue.abilityConfirmed.AddListener(OnAbilityConfirmed);
            dialogue.unitApplyAbilityProjection.AddListener(OnUnitProjected);
            dialogue.unitCancelAbilityProjection.AddListener(OnCancelUnitProjected);
        }

        protected override void Unsubscribe()
        {
            dialogue.startedMove.RemoveListener(OnStartedMove);
            dialogue.endedMove.RemoveListener(OnEndedMove);
            dialogue.unitStatChanged.RemoveListener(OnUnitStatChanged);
            dialogue.unitKilled.RemoveListener(OnUnitKilled);
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.unitSelected.RemoveListener(OnUnitSelected);
            dialogue.unitDeselected.RemoveListener(OnUnitDeselected);
            
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
            dialogue.unitApplyAbilityProjection.RemoveListener(OnUnitProjected);
            dialogue.unitCancelAbilityProjection.RemoveListener(OnCancelUnitProjected);
        }

        #endregion
        

        #region Listeners
        
        public void OnClick()
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Default)
                return;

            if (turnManager.ActingUnit == unitInfo.Unit || dialogue.SelectedUnit != null &&
                dialogue.SelectedUnit.Unit == unitInfo.Unit)
            {
                dialogue.unitDeselected.Invoke();
            }
            else
            {
                dialogue.unitSelected.Invoke(unitInfo);
            }
        }

        public void OnHoverEnter()
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Default)
                return;
            
            healthBarContainer.color = hoverHealthColour;
        }

        public void OnHoverExit()
        {
            healthBarContainer.color = defaultHealthColour;
        }

        private void OnStartedMove(GameDialogue.MoveInfo info)
        {
            OnHoverExit();
            eventTrigger.enabled = false;
            
            if (info.UnitInfo.Unit != unitInfo.Unit)
                return;

            moving = true;
        }

        private void OnEndedMove(GameDialogue.UnitInfo info)
        {
            eventTrigger.enabled = true;
            
            if (info.Unit != unitInfo.Unit)
                return;

            moving = false;
        }
        
        private void OnUnitStatChanged(GameDialogue.StatChangeInfo info)
        {
            if (info.Unit != unitInfo.Unit || info.StatType != StatTypes.Health)
                return;
            
            UpdateDamageText(info);
            UpdateHealthBar(info);
            UpdateStatDisplays();
        }

        private void OnUnitKilled(GameDialogue.UnitInfo info)
        {
            if (info != unitInfo)
                return;

            Destroy(gameObject);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            indicatorAnimator.gameObject.SetActive(info.CurrentUnitInfo.Unit == unitInfo.Unit);

            if (info.CurrentUnitInfo.Unit == unitInfo.Unit)
                indicatorImage.color = defaultIndicatorColour;
        }

        private void OnUnitSelected(GameDialogue.UnitInfo info)
        {
            if (info.Unit != unitInfo.Unit || turnManager.ActingUnit == unitInfo.Unit)
                return;

            indicatorImage.gameObject.SetActive(true);
            
            StatDisplay[] displays =
                {attackDisplay, defenceDisplay, primaryTenetDisplay, secondaryTenetDisplay};
            indicatorAnimator.SetBool(raisedId, displays.Count(d => d.gameObject.activeInHierarchy) > 0);
            
            indicatorImage.color = selectedIndicatorColour;
        }

        private void OnUnitDeselected()
        {
            indicatorImage.color = defaultIndicatorColour;
            
            if (turnManager.ActingUnit != unitInfo.Unit)
                indicatorImage.gameObject.SetActive(false);
        }
        
        private void OnAbilityConfirmed()
        {
            UpdateStatDisplays();
            UpdateHealthBar();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            UpdateStatDisplays();
            UpdateHealthBar();
        }
        
        private void OnUnitProjected(GameDialogue.ProjectedUnitInfo info)
        {
            if (info.UnitInfo != unitInfo)
                return;

            UpdateStatDisplays(info.VirtualUnit);
            UpdateHealthBarProjection(info.VirtualUnit);
        }

        private void OnCancelUnitProjected(GameDialogue.ProjectedUnitInfo info)
        {
            if (info.UnitInfo != unitInfo)
                return;
            
            UpdateStatDisplays();
            UpdateHealthBar();
        }
        
        #endregion
        
        
        #region Drawing

        internal void Assign(GameDialogue.UnitInfo info)
        {
            unitInfo = info;
            rectTransform.position = unitInfo.Unit.transform.position;
            
            UpdateHealthBar();
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
        
        private void SetHealthBarProjection(float percentage)
        {
            if (healthBarProjection != null)
                healthBarProjection.fillAmount = percentage;
                // healthBarProjection.material.SetFloat(fillId, currentHealth);
        }

        internal void UpdateHealthBarProjection(VirtualUnit virtualUnit)
        {
            float currentHealth = (float) unitInfo.Unit.HealthStat.Value / unitInfo.Unit.HealthStat.BaseValue;
            float healthAfter = (float) virtualUnit.Health.TotalValue / virtualUnit.Health.TotalBaseValue;
            SetHealthBar(healthAfter, virtualUnit.Health.TotalValue);
            SetHealthBarProjection(currentHealth);
        }

        /// <summary>
        /// Instantly update the health bar to what it's supposed to be.
        /// </summary>
        internal void UpdateHealthBar()
        {
            float percentage = (float) unitInfo.Unit.HealthStat.Value / unitInfo.Unit.HealthStat.BaseValue;
            SetHealthBar(percentage, unitInfo.Unit.HealthStat.Value);
            SetHealthBarProjection(0);
        }

        private void SetHealthBar(float percentage, int currentValue)
        {
            healthBarCurrent.material.SetFloat(fillId, percentage);
            
            if (healthText)
                healthText.text = currentValue.ToString();
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

            SetHealthBar(tCurrent, data.NewValue);
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

        /// <summary>
        /// Update the stat display of a Unit.
        /// </summary>
        /// <param name="projectedUnit">When not null, it will display the projected stat</param>
        internal void UpdateStatDisplays(VirtualUnit projectedUnit = null)
        {
            // Start with the base color
            attackDisplay.ResetTint();
            defenceDisplay.ResetTint();
            primaryTenetDisplay.ResetTint();
            secondaryTenetDisplay.ResetTint();

            if (unitInfo.Unit.AttackStat.Value == 0 && (projectedUnit == null || !projectedUnit.Attack.HasDelta))
            {
                attackDisplay.gameObject.SetActive(false);
            }
            else
            {
                attackDisplay.gameObject.SetActive(true);

                if (projectedUnit != null && projectedUnit.Attack.HasDelta)
                {
                    attackDisplay.Assign(projectedUnit.Attack.TotalValue);
                    attackDisplay.SetProjectedTint();
                }
                else
                {
                    attackDisplay.Assign(unitInfo.Unit.AttackStat.Value);
                }
            }
            
            if (unitInfo.Unit.DefenceStat.Value == 0 && (projectedUnit == null || !projectedUnit.Defence.HasDelta))
            {
                defenceDisplay.gameObject.SetActive(false);
            }
            else
            {
                defenceDisplay.gameObject.SetActive(true);

                if (projectedUnit != null && projectedUnit.Defence.HasDelta)
                {
                    defenceDisplay.Assign(projectedUnit.Defence.TotalValue);
                    defenceDisplay.SetProjectedTint();
                }
                else
                {
                    defenceDisplay.Assign(unitInfo.Unit.DefenceStat.Value);
                }
            }

            AssignTenetDisplay(0, primaryTenetDisplay, projectedUnit);
            AssignTenetDisplay(1, secondaryTenetDisplay, projectedUnit);
            RepositionStatDisplays();
        }

        private void AssignTenetDisplay(int index, TenetDisplay tenetDisplay, VirtualUnit projectedUnit = null)
        {
            TenetStatus tenetStatus = unitInfo.Unit.TenetStatuses.ElementAtOrDefault(index);
            TenetStatus projectedStatus = (projectedUnit?.TenetStatuses.ElementAtOrDefault(index))
                .GetValueOrDefault(new TenetStatus());

            if (!tenetStatus.IsEmpty || !projectedStatus.IsEmpty)
            {
                tenetDisplay.gameObject.SetActive(true);

                // Check that if there is a projected unit and there is a change
                if (projectedUnit != null && projectedStatus != tenetStatus)
                {
                    tenetDisplay.Assign(projectedStatus.StackCount);
                    tenetDisplay.SetTenet(projectedStatus.TenetType);
                    tenetDisplay.SetProjectedTint();
                }
                else
                {
                    tenetDisplay.Assign(tenetStatus.StackCount);
                    tenetDisplay.SetTenet(tenetStatus.TenetType);
                }
            }
            else
            {
                tenetDisplay.gameObject.SetActive(false);
            }
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
            
            indicatorAnimator.SetBool(raisedId, activeDisplays.Length > 0);
        }
        
        #endregion
    }
}
