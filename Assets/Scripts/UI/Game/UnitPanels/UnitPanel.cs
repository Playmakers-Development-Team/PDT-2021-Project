using System.Globalization;
using System.Linq;
using StatusEffects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    internal abstract class UnitPanel : Element
    {
        // TODO: Optimise redrawing by implementing different value changes in UIManager...
        
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


        protected IUnit selectedUnit;
        
        protected override void OnComponentAwake()
        {
            // TODO: Listening may be able to be moved to sub-classes...
            manager.turnChanged.AddListener(() => OnUnitChanged(selectedUnit));
        }

        private void OnDisable()
        {
            // TODO: Once stat listeners are implemented, add RemoveListener calls...
            
            Disabled();
        }
        
        protected virtual void Disabled() {}


        private void OnUnitChanged(IUnit unit)
        {
            if (unit != selectedUnit)
                return;
            
            Redraw();
        }
        

        protected void Redraw()
        {
            if (selectedUnit == null)
                return;

            // Unit name text
            nameText.text = selectedUnit.Name;
            
            // Render image
            renderImage.sprite = selectedUnit.Render;
            // TODO: YUCKY CAST
            renderImage.color = selectedUnit.UnitColor;
            
            // Health bar
            healthSlider.minValue = 0;
            healthSlider.maxValue = selectedUnit.Health.HealthPoints.BaseValue;
            healthSlider.value = selectedUnit.Health.HealthPoints.Value;
            healthText.text = healthSlider.value.ToString(CultureInfo.InvariantCulture);
            
            // Stat cards
            attackCard.Apply("ATT", (int) selectedUnit.Attack.Value);
            defenceCard.Apply("DEF", (int) selectedUnit.Health.Defence.Value);

            TenetStatCard[] cards = {primaryTenetCard, secondaryTenetCard};
            TenetStatus[] effects = selectedUnit.TenetStatuses.ToArray();

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
            abilityCards.Redraw(selectedUnit);
        }
    }
}
