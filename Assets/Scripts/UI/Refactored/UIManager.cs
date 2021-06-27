using System;
using Managers;
using Units;

namespace UI.Refactored
{
    [Serializable]
    public class UIManager : Manager
    {
        public readonly Event<IUnit> selectedUnit = new Event<IUnit>();
        public readonly Event deselectedUnit = new Event();

        public readonly Event<GridSelection> gridSpacesSelected = new Event<GridSelection>();
        public readonly Event gridSpacesDeselected = new Event();
        
        public readonly Event turnChanged = new Event();

        public readonly Event<IUnit> unitChanged = new Event<IUnit>();

        public readonly Event<IUnit> unitSpawned = new Event<IUnit>();

        
        public UIManager() {}
    }
}