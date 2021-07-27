using System;
using Commands;
using Managers;
using Turn.Commands;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.Beacons.Base
{
    public class UIAbilityListBeacons : MonoBehaviour
    {
        private CommandManager commandManager;
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.ListenCommand<StartTurnCommand>(cmd =>
            {
                if (cmd.Unit is PlayerUnit)
                    AddBeacons();
            });
        }

        private void Update()
        {
            if (GetComponentsInChildren<Button>() != null)
            {
                AddBeacons();
            }
        }

        private void AddBeacons()
        {
            var buttons = GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                var beacon = buttons[i].gameObject.GetComponent<UIBeacon>();
                
                if (beacon == null)
                    beacon = buttons[i].gameObject.AddComponent<UIBeacon>();

                beacon.label = (UIBeacons) i;
            }
        }
    }
}
