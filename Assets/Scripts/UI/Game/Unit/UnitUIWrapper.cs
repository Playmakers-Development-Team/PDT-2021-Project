using System;
using Commands;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using UI.Core;
using Units;
using Units.Commands;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUIWrapper : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameDialogue.UnitInfo info;

        private IUnit unit;
        private CommandManager commandManager;


        private bool startingUnit = false;
        
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
            commandManager.CatchCommand((Action<SpawnedUnitCommand>) OnUnitSpawn);

            
        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
            commandManager.CatchCommand((Action<GridReadyCommand>) OnGridReady);
            commandManager.CatchCommand((Action<SpawnedUnitCommand>) OnUnitSpawn);

        }
        
        #endregion
        
        
        #region Listeners

        private void OnGridReady(GridReadyCommand cmd)
        {

            startingUnit = true;
            
            info.SetUnit(unit);
            
            if (dialogue == null)
                return;
            
            dialogue.unitSpawned.Invoke(info);
        }
        
        private void OnUnitSpawn(SpawnedUnitCommand cmd)
        {

            if (startingUnit)
                return;
            
            info.SetUnit(unit);
            
            if (dialogue == null)
                return;
            
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
