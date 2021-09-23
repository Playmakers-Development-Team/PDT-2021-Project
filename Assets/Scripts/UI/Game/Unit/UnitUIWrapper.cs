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
        [SerializeField] private GameDialogue.UnitInfo info;

        private IUnit unit;
        private CommandManager commandManager;
        
        
        #region UIComponent

        protected override void OnComponentAwake()
        {
            unit = transform.parent.GetComponentInChildren<IUnit>();
            if (unit == null)
            {
                Debug.LogError("Could not find IUnit among parent Transform's children.");
                return;
            }
            
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.CatchCommand((Action<GridReadyCommand>) OnGridReady);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe()
        {
            commandManager.CatchCommand((Action<GridReadyCommand>) OnGridReady);
        }
        
        #endregion
        
        
        #region Listeners

        private void OnGridReady(GridReadyCommand cmd)
        {
            info.SetUnit(unit);
            
            if (dialogue == null)
                return;
            
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
