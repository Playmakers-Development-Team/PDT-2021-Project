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


        [ContextMenu("Select Test Unit")]
        private void SelectTestUnit()
        {
            if (testUnit == null)
                return;
            
            manager.selectedUnit.Invoke(testUnit);
        }

        [ContextMenu("Deselect Test Unit")]
        private void DeselectTestUnit()
        {
            manager.deselectedUnit.Invoke();
        }

        [ContextMenu("Change Test Unit")]
        private void ChangeTestUnit()
        {
            if (selectedUnit == null)
                return;
            
            selectedUnit.gameObject.name = "New Name";

            manager.unitChanged.Invoke(selectedUnit);
        }
        
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
        }
    }
}
