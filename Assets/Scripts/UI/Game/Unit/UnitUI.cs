 using System.Collections.Generic;
using Grid;
using Managers;
using UI.Core;
 using Units.Players;
 using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUI : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameObject playerUIPrefab;
        [SerializeField] private GameObject enemyUIPrefab;

        private GridManager gridManager;
        private readonly List<UnitDisplay> displays = new List<UnitDisplay>();

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            gridManager = ManagerLocator.Get<GridManager>();
        }

        protected override void OnComponentStart()
        {
            foreach (UnitDisplay display in displays)
            {
                display.transform.position =
                    gridManager.ConvertCoordinateToPosition(display.UnitInfo.Unit.Coordinate);
            }
        }

        protected override void Subscribe()
        {
            dialogue.unitSpawned.AddListener(OnUnitSpawned);
            dialogue.unitKilled.AddListener(OnUnitKilled);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSpawned.RemoveListener(OnUnitSpawned);
            dialogue.unitKilled.RemoveListener(OnUnitKilled);
        }

        private void OnUnitSpawned(GameDialogue.UnitInfo info)
        {
            UnitDisplay ui =
                Instantiate(info.Unit is PlayerUnit ? playerUIPrefab : enemyUIPrefab, transform).
                    GetComponent<UnitDisplay>();
            
            ui.Assign(info);
            displays.Add(ui);
        }

        private void OnUnitKilled(GameDialogue.UnitInfo info)
        {
            int index = displays.FindIndex(d => d.UnitInfo == info);
            if (index == -1)
                return;

            displays.RemoveAt(index);
        }
    }
}
