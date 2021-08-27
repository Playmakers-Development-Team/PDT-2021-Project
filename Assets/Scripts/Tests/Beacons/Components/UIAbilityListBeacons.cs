using System.Linq;
using Commands;
using Managers;
using Turn.Commands;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.Beacons.Components
{
    public class UIAbilityListBeacons : MonoBehaviour
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
            // TODO: Replace this with a more efficient method
            if (GetComponentsInChildren<Button>() != null)
            {
                AddBeacons();
            }
        }

        private void AddBeacons()
        {
            // TODO: There's probably a better way to do this
            var buttons = GetComponentsInChildren<Transform>().Where(t => t.parent == transform).ToList();

            for (int i = 0; i < buttons.Count; i++)
            {
                var beacon = buttons[i].gameObject.GetComponent<UIBeacon>();
                
                if (beacon == null)
                    beacon = buttons[i].gameObject.AddComponent<UIBeacon>();
            
                beacon.label = (UIBeacons) i;
            }
        }
    }
}
