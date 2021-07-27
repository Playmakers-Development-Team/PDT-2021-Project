using Grid.GridObjects;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUIWrapper : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private GameDialogue.UnitInfo info;
        
        
        #region UIComponent

        protected override void OnComponentAwake()
        {
            if (!(unitGridObject is IUnit unit))
                return;
            
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
    }
}
