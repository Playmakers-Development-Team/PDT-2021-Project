using Grid.GridObjects;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.AbilityLoadout.Unit
{
    public class UnitLoadoutUIWrapper : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private AbilityLoadoutDialogue.UnitInfo info;
        
        
        #region UIComponent

        protected override void Subscribe()
        {
            if (!(unitGridObject is IUnit unit))
                return;
            
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }

        protected override void Unsubscribe() {}
        
        #endregion
    }
}
