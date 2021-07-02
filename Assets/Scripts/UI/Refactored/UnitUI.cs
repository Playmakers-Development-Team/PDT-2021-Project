using GridObjects;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Refactored
{
    public class UnitUI : Element
    {
        [SerializeField] private GridObject unitGridObject;

        private IUnit unit;


        protected override void OnAwake()
        {
            unit = unitGridObject as IUnit;
            
            if (unit == null)
                DestroyImmediate(gameObject);
        }

        
        public void OnClick() => manager.selectedUnit.Invoke(unit);
    }
}
