using System.Linq;
using Cysharp.Threading.Tasks;
using TenetStatuses;
using TMPro;
using UI.Core;
using UI.Game.UnitPanels.Abilities;
using UI.Game.UnitPanels.Stats;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels
{
    internal abstract class UnitPanel : DialogueComponent<GameDialogue>
    {
        [SerializeField] protected RawImage renderImage;
        
        [SerializeField] protected TextMeshProUGUI currentHealthText;
        [SerializeField] protected TextMeshProUGUI baseHealthText;
        
        [SerializeField] protected ValueStatCard attackCard;
        [SerializeField] protected ValueStatCard defenceCard;
        
        [SerializeField] protected TenetStatCard primaryTenetCard;
        [SerializeField] protected TenetStatCard secondaryTenetCard;

        [SerializeField] protected AbilityList abilityCards;
        
        [Header("Health Bar")]
        
        [SerializeField] protected Image healthFillImage;
        [SerializeField] protected Image healthBackgroundImage;
        [SerializeField] protected Image healthBorderImage;
        
        [SerializeField] protected Color playerFillColour;
        [SerializeField] protected Color playerBackgroundColour;
        [SerializeField] protected Color playerBorderColour;
        
        [SerializeField] protected Color enemyFillColour;
        [SerializeField] protected Color enemyBackgroundColour;
        [SerializeField] protected Color enemyBorderColour;

        [Header("Transition")]
        
        [SerializeField] private Animator animator;
        [SerializeField] private float delay;
        
        protected GameDialogue.UnitInfo unitInfo;
        
        private static readonly int fillId = Shader.PropertyToID("_Fill");
        private static readonly int dividerColorId = Shader.PropertyToID("_DividerColor");
        
        private static readonly int promoted = Animator.StringToHash("promoted");
        private static readonly int demoted = Animator.StringToHash("demoted");


        #region DialogueComponent

        protected override void OnComponentStart()
        {
            if (manager.Peek() == dialogue)
                TransitionIn();
        }

        protected override void OnComponentAwake()
        {
            healthFillImage.material = Instantiate(healthFillImage.material);
        }

        protected override void Subscribe()
        {
            dialogue.promoted.AddListener(OnPromoted);
            dialogue.demoted.AddListener(OnDemoted);
        }

        protected override void Unsubscribe()
        {
            dialogue.promoted.RemoveListener(OnPromoted);
            dialogue.demoted.RemoveListener(OnDemoted);
        }

        #endregion


        private async void TransitionIn()
        {
            await UniTask.Delay((int) (delay * 1000.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
            OnPromoted();
        }
        

        #region Drawing
        
        // TODO: Optimise redrawing by implementing different value changes in GameDialogue...
        protected void Redraw()
        {
            if (unitInfo == null)
                return;
            
            // Render image
            renderImage.texture = unitInfo.ProfileCropInfo.Image;
            renderImage.color = unitInfo.ProfileCropInfo.Colour;
            renderImage.uvRect = unitInfo.ProfileCropInfo.UVRect;
            
            // Health bar
            bool isPlayer = unitInfo.Unit is PlayerUnit;

            healthBackgroundImage.color =
                isPlayer ? playerBackgroundColour : enemyBackgroundColour;
            
            healthBorderImage.color = isPlayer ? playerBorderColour : enemyBorderColour;
            
            healthFillImage.color = isPlayer ? playerFillColour : enemyFillColour;
            healthFillImage.material.SetColor(dividerColorId,
                isPlayer ? playerBorderColour : enemyBorderColour);
            
            healthFillImage.material.SetFloat(fillId,
                unitInfo.Unit.HealthStat.Value / (float) unitInfo.Unit.HealthStat.BaseValue);
            
            currentHealthText.text = unitInfo.Unit.HealthStat.Value.ToString();
            baseHealthText.text = unitInfo.Unit.HealthStat.BaseValue.ToString();
            
            // Stat cards
            attackCard.Apply(unitInfo.Unit.AttackStat.Value);
            defenceCard.Apply(unitInfo.Unit.DefenceStat.Value);

            TenetStatCard[] cards = {primaryTenetCard, secondaryTenetCard};
            TenetStatus[] effects = unitInfo.Unit.TenetStatuses.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                if (i >= effects.Length)
                {
                    cards[i].Hide();
                    continue;
                }
                
                cards[i].Show();
                cards[i].Apply(Settings.GetTenetIcon(effects[i].TenetType), effects[i].StackCount);
            }
            
            // Ability cards
            // TODO: This will soon become Unit.AbilityList or something like that...
            abilityCards.Redraw(unitInfo.Unit);
        }
        
        #endregion
        
        
        #region Listeners

        private void OnPromoted()
        {
            animator.SetTrigger(promoted);
        }

        private void OnDemoted()
        {
            animator.SetTrigger(demoted);
        }

        #endregion
    }
}
