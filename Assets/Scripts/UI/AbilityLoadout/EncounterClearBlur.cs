using System;
using Commands;
using Managers;
using UI.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout
{
    public class EncounterClearBlur : MonoBehaviour
    {
        private CommandManager commandManager;
        private Image blurImage;

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();

            blurImage = GetComponent<Image>();
        }
        
        private void OnEnable()
        {
            commandManager.ListenCommand((Action<SpawnAbilityLoadoutUICommand>) ShowBlur);
            commandManager.ListenCommand((Action<SpawnAbilityUpgradeUICommand>) ShowBlur);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) ShowBlur);
            commandManager.UnlistenCommand((Action<SpawnAbilityUpgradeUICommand>) ShowBlur);
        }

        private void ShowBlur(SpawnAbilityLoadoutUICommand cmd)
        {
            blurImage.enabled = true;
        }
        
        private void ShowBlur(SpawnAbilityUpgradeUICommand cmd)
        {
            blurImage.enabled = true;
        }
    }
}
