using Units;

namespace UI.Refactored
{
    public class UnitPanel : Element
    {
        private IUnit selectedUnit;

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
        }

        private void OnUnitSelected(IUnit unit)
        {
            selectedUnit = unit;
            Refresh();
        }

        private void OnUnitDeselected()
        {
            selectedUnit = null;
            Refresh();
        }

        private void OnUnitChanged(IUnit unit)
        {
            if (!unit.Equals(selectedUnit))
                return;

            Refresh();
        }

        protected override void Refresh()
        {
            
        }
    }
}
