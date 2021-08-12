using Commands;
using Managers;
using Turn.Commands;
using UnityEngine;

namespace Playtest
{
    [ExecuteAlways]
    public class Playtest : MonoBehaviour
    {
        [SerializeField] private PlaytestData data;
        
        private DataCollection collection;

        #region Managers

        private CommandManager commandManager;

        #endregion
        
        // TODO: Timer stuff should be moved to another class.
        private Timer roundTimer;
        private Timer turnTimer;
        private Timer encounterTimer;

        #region MonoBehaviour

        private void Awake()
        {
            if (!Application.isPlaying)
                return;
            
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                DataPosting.PostAll();
                return;
            }
            
            commandManager.ListenCommand<TurnQueueCreatedCommand>(OnTurnQueueCreated);
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(OnNoRemainingEnemyUnits);
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(OnNoRemainingPlayerUnits);
            commandManager.ListenCommand<PrepareRoundCommand>(OnPrepareRound);
            commandManager.ListenCommand<EndTurnCommand>(OnEndTurn);

            collection.OnEnable();
        }

        private void OnEndTurn(EndTurnCommand cmd)
        {
            collection.OnEndTurn(cmd, turnTimer.Elapsed);
            turnTimer.Reset();
        }

        private void OnPrepareRound(PrepareRoundCommand cmd)
        {
            collection.OnPrepareRound(cmd, roundTimer.Elapsed);
            roundTimer.Reset();
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand<TurnQueueCreatedCommand>(OnTurnQueueCreated);
            commandManager.UnlistenCommand<NoRemainingEnemyUnitsCommand>(OnNoRemainingEnemyUnits);
            commandManager.UnlistenCommand<NoRemainingPlayerUnitsCommand>(OnNoRemainingPlayerUnits);
            commandManager.UnlistenCommand<PrepareRoundCommand>(OnPrepareRound);
            commandManager.UnlistenCommand<EndTurnCommand>(OnEndTurn);

            collection.OnDisable();
        }

        #endregion

        private void OnTurnQueueCreated(TurnQueueCreatedCommand cmd) => InitialiseStats();

        private void OnNoRemainingEnemyUnits(NoRemainingEnemyUnitsCommand cmd) => EndGame(true);

        private void OnNoRemainingPlayerUnits(NoRemainingPlayerUnitsCommand cmd) => EndGame(false);

        private void InitialiseStats()
        {
            // TODO: Could we pass the active state into these constructors?
            roundTimer = gameObject.AddComponent<Timer>();
            turnTimer = gameObject.AddComponent<Timer>();
            encounterTimer = gameObject.AddComponent<Timer>();

            roundTimer.StartTimer();
            turnTimer.StartTimer();
            encounterTimer.StartTimer();
            
            collection = new DataCollection(data);
            
            data = ScriptableObject.CreateInstance<PlaytestData>();

            collection.InitialiseStats();

            DataPosting.InitialiseStatsEntries(data);
        }

        /// <summary>
        /// All data that is processed during the game will be calculated in this function
        /// </summary>
        private void EndGame(bool playerWin)
        {
            roundTimer.StopTimer();
            turnTimer.StopTimer();
            encounterTimer.StopTimer();
            
            collection.EndGame(playerWin, encounterTimer.Elapsed);

            DataPosting.EndGameEntries(data);
        }
    }
}
