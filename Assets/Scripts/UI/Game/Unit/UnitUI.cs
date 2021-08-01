using UI.Core;
using UnityEngine;

namespace UI.Game.Unit
{
    public class UnitUI : DialogueComponent<GameDialogue>
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
            UnitDisplay ui = Instantiate(unitUIPrefab, transform).GetComponent<UnitDisplay>();
            ui.Assign(info);
        }
    }
}
