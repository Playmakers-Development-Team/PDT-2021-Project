using System;
using Commands;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.AbilityLoadout.Unit
{
    public class UnitLoadoutUIWrapper : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private AbilityLoadoutDialogue.UnitInfo info;
        
        private CommandManager commandManager;
        
        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.CatchCommand((Action<GridObjectsReadyCommand>) OnGridObjectsReady);
        }
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners

        private void OnGridObjectsReady(GridObjectsReadyCommand cmd)
        {
            if (!(unitGridObject is IUnit unit))
                return;
            
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
