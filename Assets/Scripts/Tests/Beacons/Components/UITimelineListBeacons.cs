using System.Linq;
using Commands;
using Managers;
using Turn.Commands;
using UI.Game.Timeline;
using Units.Players;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

namespace Tests.Beacons.Components
{
    public class UITimelineListBeacons : MonoBehaviour
    {
        private CommandManager commandManager;
        private void Awake()
        {
            // commandManager = ManagerLocator.Get<CommandManager>();
            // commandManager.ListenCommand<StartTurnCommand>(cmd =>
            // {
            //     if (cmd.Unit is PlayerUnit)
            //         AddBeacons();
            // });
        }

        private void Update()
        {
            // TODO: Make a more efficient command or something
            if (GetComponentsInChildren<TimelinePortrait>() != null)
            {
                AddBeacons();
            }
        }

        private void AddBeacons()
        {
            var buttons = GetComponentsInChildren<TimelinePortrait>().ToList();

            // Removes divider
            var remove = buttons.Find(d => d.gameObject.name == "Divider(Clone)");
            buttons.Remove(remove);
            
            for (var index = 0; index < buttons.Count; index++)
            {
                var beacon = buttons[index].gameObject.GetComponent<UITimelineBeacon>();

                if (beacon == null)
                    beacon = buttons[index].gameObject.AddComponent<UITimelineBeacon>();
                
                beacon.label = (UITimelineBeacons) index;
            }
        }
    }
}
