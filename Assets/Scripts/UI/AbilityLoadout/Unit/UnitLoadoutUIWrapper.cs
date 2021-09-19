using System;
using System.Collections.Generic;
using Commands;
using Grid.GridObjects;
using Managers;
using UI.Commands;
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
            commandManager.CatchCommand((Action<AbilityLoadoutReadyCommand>) OnAbilityLoadoutReady);
        }
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners

        private void OnAbilityLoadoutReady(AbilityLoadoutReadyCommand cmd)
        {
            if (!(unitGridObject is IUnit unit))
                return;

            // Assign abilities
            List<AbilityLoadoutDialogue.AbilityInfo> abilityInfo = new List<AbilityLoadoutDialogue.AbilityInfo>();
            for (int i = 0; i < unit.Abilities.Count; ++i)
            {
                abilityInfo.Add(dialogue.GetInfo(unit.Abilities[i]));
            }
            
            info.SetAbilityInfo(abilityInfo);

            // Assign unit
            info.SetUnit(unit);
            dialogue.unitSpawned.Invoke(info);
        }
        
        #endregion
    }
}
