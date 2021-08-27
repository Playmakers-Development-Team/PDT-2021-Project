using System;
using Commands;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUIWrapper : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GridObject unitGridObject;
        [SerializeField] private GameDialogue.UnitInfo info;

        private CommandManager commandManager;
        
        
        #region UIComponent

        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.CatchCommand((Action<GridReadyCommand>) OnGridReady);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Listeners

        private void OnGridReady(GridReadyCommand cmd)
        {
            if (!(unitGridObject is IUnit unit))
                return;
            
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
