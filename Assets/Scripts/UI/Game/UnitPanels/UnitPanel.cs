using System.Globalization;
using System.Linq;
using TMPro;
using TenetStatuses;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    internal abstract class UnitPanel : UIComponent<GameDialogue>
    {
        // TODO: Optimise redrawing by implementing different value changes in GameDialogue...
        
        [SerializeField] protected Canvas canvas;
        
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected Image renderImage;

        [SerializeField] protected Slider healthSlider;
        [SerializeField] protected TextMeshProUGUI healthText;
        
        [SerializeField] protected ValueStatCard attackCard;
        [SerializeField] protected ValueStatCard defenceCard;
        
        [SerializeField] protected TenetStatCard primaryTenetCard;
        [SerializeField] protected TenetStatCard secondaryTenetCard;

        [SerializeField] protected AbilityList abilityCards;
        
        protected GameDialogue.UnitInfo unitInfo;
        

        protected void Redraw()
        {
            if (unitInfo == null)
                return;
            
            // Unit name text
            nameText.text = unitInfo.Unit.Name;
            
            // Render image
            renderImage.sprite = unitInfo.Render;
            // TODO: YUCKY CAST
            renderImage.color = unitInfo.Color;
            
            // Health bar
            healthSlider.minValue = 0;
            healthSlider.maxValue = unitInfo.Unit.Health.HealthPoints.BaseValue;
            healthSlider.value = unitInfo.Unit.Health.HealthPoints.Value;
            healthText.text = healthSlider.value.ToString(CultureInfo.InvariantCulture);
            
            // Stat cards
            attackCard.Apply("ATT", (int) unitInfo.Unit.Attack.Value);
            defenceCard.Apply("DEF", (int) unitInfo.Unit.Health.Defence.Value);

            TenetStatCard[] cards = {primaryTenetCard, secondaryTenetCard};
            TenetStatus[] effects = unitInfo.Unit.TenetStatuses.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                if (i >= effects.Length)
                {
                    // TODO: Create Hide() and Show() methods in IStatCard rather than disabling GameObjects here...
                    cards[i].gameObject.SetActive(false);
                    continue;
                }
                
                cards[i].gameObject.SetActive(true);
                cards[i].Apply(effects[i].TenetType, effects[i].StackCount);
            }
            
            // Ability cards
            // TODO: This will soon become Unit.AbilityList or something like that...
            abilityCards.Redraw(unitInfo.Unit);
        }
    }
}
