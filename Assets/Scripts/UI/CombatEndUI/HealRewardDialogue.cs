using System;
using System.Collections.Generic;
using Commands;
using Cysharp.Threading.Tasks;
using Game;
using Game.Commands;
using Managers;
using UI.Core;
using Units.Players;
using UnityEngine;

namespace UI.CombatEndUI
{
    public class HealRewardDialogue : Dialogue
    {
        [SerializeField] private float delayBeforeNextEncounter = 4f;
        
        protected readonly List<LoadoutUnitInfo> units = new List<LoadoutUnitInfo>();
        internal readonly Event<LoadoutUnitInfo> unitSpawned = new Event<LoadoutUnitInfo>();

        private CommandManager commandManager;
        private PlayerManager playerManager;
    
        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();

            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
        
            unitSpawned.AddListener(info =>
            {
                if (info.Unit is PlayerUnit)
                    units.Add(info);
            });
        }

        protected override void OnClose() {}

        protected override void OnPromote()
        {
            Heal();
        }

        protected override void OnDemote() {}

        private async void Heal()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeNextEncounter));
            playerManager.HealAllPlayers();
            commandManager.ExecuteCommand(new EndEncounterCommand());
        }
    }
}
