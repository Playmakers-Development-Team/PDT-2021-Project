using System.Globalization;
using StatusEffects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Refactored
{
    public abstract class UnitPanel : Element
    {
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


        protected override void OnAwake()
        {
            manager.unitChanged.AddListener(OnUnitChanged);
        }

        private void OnDisable()
        {
            manager.unitChanged.RemoveListener(OnUnitChanged);
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
            {
                Debug.LogWarning("Attempted to redraw Unit UI without a selected Unit");
                return;
            }

            // Unit name text
            nameText.text = selectedUnit.gameObject.name;
            
            // Render image
            renderImage.sprite = selectedUnit.Render;
            
            // Health bar
            healthSlider.minValue = 0;
            healthSlider.maxValue = selectedUnit.Health.HealthPoints.BaseValue;
            healthSlider.value = selectedUnit.Health.HealthPoints.Value;
            healthText.text = healthSlider.value.ToString(CultureInfo.InvariantCulture);
            
            // Stat cards
            // TODO: What hte fuck even si this
            attackCard.Apply("ATT", (int) selectedUnit.DealDamageModifier.Value);
            defenceCard.Apply("DEF", (int) selectedUnit.Health.TakeDamageModifier.Value);
            
            primaryTenetCard.Apply(selectedUnit.Tenet, selectedUnit.GetTenetStatusEffectCount(selectedUnit.Tenet));
            // TODO: Is there even secondary tenets in the game??
            secondaryTenetCard.Apply((TenetType) (-1), -1);
            
            // Ability cards
            abilityCards.Redraw(selectedUnit);
        }
    }
}
