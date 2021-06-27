using System.Globalization;
using StatusEffects;
using TMPro;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Refactored
{
    public class UnitPanel : Element
    {
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image renderImage;

        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [SerializeField] private ValueStatCard attackCard;
        [SerializeField] private ValueStatCard defenceCard;
        
        [SerializeField] private TenetStatCard primaryTenetCard;
        [SerializeField] private TenetStatCard secondaryTenetCard;

        [SerializeField] private AbilityList abilityCards;


        private IUnit selectedUnit;

        
        #region MonoBehaviour functions

        protected override void OnAwake()
        {
            manager.selectedUnit.AddListener(OnUnitSelected);
            manager.deselectedUnit.AddListener(OnUnitDeselected);
            manager.unitChanged.AddListener(OnUnitChanged);

            Hide();
        }

        private void OnDisable()
        {
            manager.selectedUnit.RemoveListener(OnUnitSelected);
            manager.deselectedUnit.RemoveListener(OnUnitDeselected);
            manager.unitChanged.RemoveListener(OnUnitChanged);
        }

        #endregion
        
        
        #region Event functions
        
        private void OnUnitSelected(IUnit unit)
        {
            selectedUnit = unit;
            Show();
            Redraw();
        }

        private void OnUnitDeselected()
        {
            selectedUnit = null;
            Hide();
        }

        private void OnUnitChanged(IUnit unit)
        {
            if (unit != selectedUnit)
                return;
            
            Redraw();
        }

        #endregion
        
        private void Hide()
        {
            canvas.enabled = false;
        }

        private void Show()
        {
            canvas.enabled = true;
        }

        private void Redraw()
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
