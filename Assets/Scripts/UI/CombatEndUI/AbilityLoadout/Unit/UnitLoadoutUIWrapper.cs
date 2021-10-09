using System;
using System.Collections.Generic;
using Commands;
using Managers;
using UI.Commands;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.CombatEndUI.AbilityLoadout.Unit
{
    public class UnitLoadoutUIWrapper : DialogueComponent<AbilityRewardDialogue>
    {
        [SerializeField] private LoadoutUnitInfo info;

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
            commandManager.CatchCommand((Action<AbilityRewardDialogueReadyCommand>) OnAbilityRewardDialogueReady);
        }
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        #endregion
        
        #region Listeners

        private void OnAbilityRewardDialogueReady(AbilityRewardDialogueReadyCommand cmd)
        {
            // Assign abilities
            List<LoadoutAbilityInfo> abilityInfo = new List<LoadoutAbilityInfo>();
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
