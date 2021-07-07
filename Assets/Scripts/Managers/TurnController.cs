using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class TurnController : MonoBehaviour
    {
        /// <summary>
        /// The Transform for the timeline, used as the parent to instantiate the unit cards.
        /// </summary>
        [SerializeField] private Transform timeline;

        /// <summary>
        /// The prefab for the unit card.
        /// </summary>
        [SerializeField] private GameObject unitCardPrefab;

        /// <summary>
        /// The prefab for the current turn indicator.
        /// </summary>
        [SerializeField] private GameObject currentTurnIndicatorPrefab;

        /// <summary>
        /// A list of all the unit cards shown in the timeline.
        /// </summary>
        [SerializeField] private List<UnitCard> allUnitCards;

        [SerializeField]

        /// <summary>
        /// The GameObject for the current turn indicator.
        /// </summary>
        private GameObject currentTurnIndicator;
        
        /// <summary>
        /// A reference to the TurnManager.
        /// </summary>
        private TurnManager turnManager;

        private void Awake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.CatchCommand<PlayerUnitsReadyCommand, EnemyUnitsReadyCommand>(
                (cmd1, cmd2) =>
                {
                    SetupTurnQueue();
                });
        }

        /// <summary>
        /// Sets up the initial timeline at the start of the game.
        /// </summary>
        private void SetupTurnQueue()
        {
            turnManager.SetupTurnQueue();
        }
    }
}
