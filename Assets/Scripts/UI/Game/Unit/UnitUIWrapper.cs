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
            unit = GetComponentInParent<IUnit>();

            if (unit == null)
            {
                Debug.LogError("Did not find IUnit on any parent GameObjects.");
                return;
            }
            
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.CatchCommand((Action<GridReadyCommand>) OnGridReady);
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        
        #region Listeners

        private void OnGridReady(GridReadyCommand cmd)
        {
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
