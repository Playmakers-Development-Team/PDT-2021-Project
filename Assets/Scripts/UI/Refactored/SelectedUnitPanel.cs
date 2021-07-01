using Units;

namespace UI.Refactored
{
    public class SelectedUnitPanel : UnitPanel
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            
            manager.selectedUnit.AddListener(OnUnitSelected);
            manager.deselectedUnit.AddListener(OnUnitDeselected);

            Hide();
        }

        protected override void Disabled()
        {
            manager.selectedUnit.RemoveListener(OnUnitSelected);
            manager.deselectedUnit.RemoveListener(OnUnitDeselected);
        }

        
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
        
        
        private void Hide() => canvas.enabled = false;

        private void Show() => canvas.enabled = true;
    }
}
