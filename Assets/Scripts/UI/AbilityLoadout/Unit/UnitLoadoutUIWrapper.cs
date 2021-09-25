using System;
using System.Collections.Generic;
using Commands;
using Grid.GridObjects;
using Managers;
using UI.Commands;
using UI.Core;
using UI.Game.Unit;
using Units;
using UnityEngine;

namespace UI.AbilityLoadout.Unit
{
    public class UnitLoadoutUIWrapper : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private AbilityLoadoutDialogue.UnitInfo info;

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
            commandManager.CatchCommand((Action<AbilityLoadoutReadyCommand>) OnAbilityLoadoutReady);
        }
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners

        private void OnAbilityLoadoutReady(AbilityLoadoutReadyCommand cmd)
        {
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
