using Units;

namespace UI
{
    internal class SelectedUnitPanel : UnitPanel
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            
            manager.unitSelected.AddListener(OnUnitSelected);
            manager.unitDeselected.AddListener(OnUnitDeselected);

            Hide();
        }

        protected override void Disabled()
        {
            manager.unitSelected.RemoveListener(OnUnitSelected);
            manager.unitDeselected.RemoveListener(OnUnitDeselected);
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
