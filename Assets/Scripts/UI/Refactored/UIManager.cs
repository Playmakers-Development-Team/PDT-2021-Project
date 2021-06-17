using System.Collections.Generic;
using Managers;
using Units;
using UnityEngine;

namespace UI.Refactored
{
    public class UIManager : Manager
    {
        public readonly Event<IUnit> selectedUnit = new Event<IUnit>();
        public readonly Event deselectedUnit = new Event();

        public readonly Event<IEnumerable<Vector2Int>> gridSpacesSelected = new Event<IEnumerable<Vector2Int>>();
        public readonly Event gridSpacesDeselected = new Event();
        
        public readonly Event turnChanged = new Event();

        public readonly Event<IUnit> unitChanged = new Event<IUnit>();

        
        public UIManager()
        {
        }
    }
}