using Grid;
using Managers;
using UI.Core;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUI : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameObject unitUIPrefab;

        private GridManager gridManager;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            gridManager = ManagerLocator.Get<GridManager>();
        }

        protected override void Subscribe()
        {
            dialogue.unitSpawned.AddListener(OnUnitSpawned);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSpawned.RemoveListener(OnUnitSpawned);
        }

        private void OnUnitSpawned(GameDialogue.UnitInfo info)
        {
            UnitDisplay ui = Instantiate(unitUIPrefab, gridManager.ConvertCoordinateToPosition(info.Unit.Coordinate), Quaternion.identity, transform).
                GetComponent<UnitDisplay>();
            ui.Assign(info);
        }
    }
}
