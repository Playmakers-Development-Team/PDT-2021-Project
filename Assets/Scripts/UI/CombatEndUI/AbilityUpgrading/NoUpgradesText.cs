using System;
using Commands;
using Managers;
using TMPro;
using UI.Commands;
using UnityEngine;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class NoUpgradesText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI noUpgradesText;

        private CommandManager commandManager;

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.ListenCommand((Action<NoUpgradesCommand>) OnEnableText);
            commandManager.ListenCommand((Action<UpgradesAvailableCommand>) OnDisableText);
        }

        private void OnEnableText(NoUpgradesCommand cmd)
        {
            noUpgradesText.enabled = true;
        }

        private void OnDisableText(UpgradesAvailableCommand cmd)
        {
            noUpgradesText.enabled = false;
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<NoUpgradesCommand>) OnEnableText);
            commandManager.UnlistenCommand((Action<UpgradesAvailableCommand>) OnDisableText);
        }
    }
}
