 using System.Collections.Generic;
 using Commands;
 using Cysharp.Threading.Tasks;
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

        [Header("Transition")]
        
        [SerializeField] private Animator animator;
        [SerializeField] private float delay;

        private GridManager gridManager;
        private CommandManager commandManager;
        private readonly List<UnitDisplay> displays = new List<UnitDisplay>();
        
        private static readonly int promoted = Animator.StringToHash("promoted");
        private static readonly int demoted = Animator.StringToHash("demoted");

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected override void OnComponentStart()
        {
            foreach (UnitDisplay display in displays)
            {
                display.transform.position =
                    gridManager.ConvertCoordinateToPosition(display.UnitInfo.Unit.Coordinate);
            }

            if (manager.Peek() == dialogue)
                TransitionIn();
        }

        protected override void Subscribe()
        {
            dialogue.unitSpawned.AddListener(OnUnitSpawned);
            dialogue.unitKilled.AddListener(OnUnitKilled);
            
            dialogue.promoted.AddListener(OnPromoted);
            dialogue.demoted.AddListener(OnDemoted);
        }

        protected override void Unsubscribe()
        {
            dialogue.unitSpawned.RemoveListener(OnUnitSpawned);
            dialogue.unitKilled.RemoveListener(OnUnitKilled);
            
            dialogue.promoted.RemoveListener(OnPromoted);
            dialogue.demoted.RemoveListener(OnDemoted);
        }

        private async void TransitionIn()
        {
            await UniTask.Delay((int) (delay * 1000.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
            OnPromoted();
        }

        private void OnPromoted() => animator.SetTrigger(promoted);

        private void OnDemoted() => animator.SetTrigger(demoted);

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
