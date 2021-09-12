using Commands;
using Managers;
using Turn.Commands;
using UnityEngine;

namespace Playtest
{
    [ExecuteAlways]
    public class Playtest : MonoBehaviour
    {
#if !UNITY_EDITOR
        // When not in the editor, always have playtest data enabled
        private const bool canRecordPlaytestData = true;
#else
        [Tooltip("Determines whether or not it will record the data from the game")]
        [SerializeField] private bool canRecordPlaytestData;
#endif

        [SerializeField] private PlaytestData data;
        
        private DataCollection collection;

        #region Managers

        private CommandManager commandManager;

        #endregion
        
        private Timer roundTimer;
        private Timer turnTimer;
        private Timer encounterTimer;

        #region MonoBehaviour

        private void Awake()
        {
            if (!Application.isPlaying || !canRecordPlaytestData)
                return;
            
            commandManager = ManagerLocator.Get<CommandManager>();
            
            collection = new DataCollection(data);
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;
           
            commandManager.ListenCommand<TurnQueueCreatedCommand>(OnTurnQueueCreated);
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(OnNoRemainingEnemyUnits);
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(OnNoRemainingPlayerUnits);
            commandManager.ListenCommand<PrepareRoundCommand>(OnPrepareRound);
            commandManager.ListenCommand<EndTurnCommand>(OnEndTurn);

            collection.OnEnable();
        }

        private void OnDisable()
        {
            
            if (!canRecordPlaytestData)
                return;

            if (!Application.isPlaying)
                return;
            
            DataPosting.PostAll(data);
            
            commandManager.UnlistenCommand<TurnQueueCreatedCommand>(OnTurnQueueCreated);
            commandManager.UnlistenCommand<NoRemainingEnemyUnitsCommand>(OnNoRemainingEnemyUnits);
            commandManager.UnlistenCommand<NoRemainingPlayerUnitsCommand>(OnNoRemainingPlayerUnits);
            commandManager.UnlistenCommand<PrepareRoundCommand>(OnPrepareRound);
            commandManager.UnlistenCommand<EndTurnCommand>(OnEndTurn);

            collection.OnDisable();
        }

        #endregion

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

        private void OnTurnQueueCreated(TurnQueueCreatedCommand cmd) => InitialiseStats();

        private void OnNoRemainingEnemyUnits(NoRemainingEnemyUnitsCommand cmd) => EndGame(true);

        private void OnNoRemainingPlayerUnits(NoRemainingPlayerUnitsCommand cmd) => EndGame(false);

        private void InitialiseStats()
        {
            roundTimer = gameObject.AddComponent<Timer>();
            turnTimer = gameObject.AddComponent<Timer>();
            encounterTimer = gameObject.AddComponent<Timer>();

            roundTimer.StartTimer();
            turnTimer.StartTimer();
            encounterTimer.StartTimer();
            
            data = ScriptableObject.CreateInstance<PlaytestData>();

            collection.Data = data;

            collection.InitialiseStats();
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
        }
    }
}
