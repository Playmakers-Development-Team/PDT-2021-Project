using UI.Core;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitController : UIComponent<GameDialogue>
    {
        [SerializeField] private GameObject unitUIPrefab;
        
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
            UnitUI ui = Instantiate(unitUIPrefab, transform).GetComponent<UnitUI>();
            ui.Assign(info);
        }
    }
}
