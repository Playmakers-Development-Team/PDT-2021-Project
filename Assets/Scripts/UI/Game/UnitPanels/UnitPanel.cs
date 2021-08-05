using System.Linq;
using TenetStatuses;
using TMPro;
using UI.Core;
using UI.Game.UnitPanels.Abilities;
using UI.Game.UnitPanels.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels
{
    internal abstract class UnitPanel : DialogueComponent<GameDialogue>
    {
        [SerializeField] protected RawImage renderImage;

        [SerializeField] protected Image healthImage;
        [SerializeField] protected TextMeshProUGUI currentHealthText;
        [SerializeField] protected TextMeshProUGUI baseHealthText;
        
        [SerializeField] protected ValueStatCard attackCard;
        [SerializeField] protected ValueStatCard defenceCard;
        
        [SerializeField] protected TenetStatCard primaryTenetCard;
        [SerializeField] protected TenetStatCard secondaryTenetCard;

        [SerializeField] protected AbilityList abilityCards;

        [Header("Tenet Icons")]
        
        [SerializeField] protected Sprite passion;
        [SerializeField] protected Sprite apathy;
        [SerializeField] protected Sprite pride;
        [SerializeField] protected Sprite humility;
        [SerializeField] protected Sprite joy;
        [SerializeField] protected Sprite sorrow;
        
        protected GameDialogue.UnitInfo unitInfo;
        
        private static readonly int fillId = Shader.PropertyToID("_Fill");


        #region DialogueComponent

        protected override void OnComponentAwake()
        {
            healthImage.material = Instantiate(healthImage.material);
        }

        #endregion
        

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
            healthImage.material.SetFloat(fillId, unitInfo.Unit.HealthStat.Value / (float) unitInfo.Unit.HealthStat.BaseValue);
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
                cards[i].Apply(GetTenetIcon(effects[i].TenetType), effects[i].StackCount);
            }
            
            // Ability cards
            // TODO: This will soon become Unit.AbilityList or something like that...
            abilityCards.Redraw(unitInfo.Unit);
        }

        private Sprite GetTenetIcon(TenetType tenet)
        {
            return tenet switch
            {
                TenetType.Passion => passion,
                TenetType.Apathy => apathy,
                TenetType.Pride => pride,
                TenetType.Humility => humility,
                TenetType.Joy => joy,
                TenetType.Sorrow => sorrow,
                _ => null
            };
        }
        
        #endregion
    }
}
