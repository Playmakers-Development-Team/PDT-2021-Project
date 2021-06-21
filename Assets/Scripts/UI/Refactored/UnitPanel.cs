using TMPro;
using Units;
using UnityEngine;

namespace UI.Refactored
{
    public class UnitPanel : Element
    {
        [SerializeField] private Canvas canvas;
        
        [SerializeField] private TextMeshProUGUI nameText;
        
        [SerializeField] private StatCard healthCard;
        [SerializeField] private StatCard defenceCard;
        
        [SerializeField] private StatCard primaryTenetCard;
        [SerializeField] private StatCard secondaryTenetCard;

        [SerializeField] private AbilityCard abilityCards;

        [Space]
        
        [SerializeField] private PlayerUnit testUnit;

        private IUnit selectedUnit;

        
        #region MonoBehaviour functions

        protected override void OnAwake()
        {
            manager.selectedUnit.AddListener(OnUnitSelected);
            manager.deselectedUnit.AddListener(OnUnitDeselected);
            manager.unitChanged.AddListener(OnUnitChanged);
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
            Redraw();
        }

        #endregion


        [ContextMenu("Assign Test Unit")]
        private void AssignTestUnit()
        {
            manager.selectedUnit.Invoke(testUnit);
        }
        
        private void Hide()
        {
            
        }

        private void Show()
        {
            
        }

        private void Redraw()
        {
            
        }
    }
}
