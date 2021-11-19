using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Cysharp.Threading.Tasks;
using Grid;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VFX.VFX
{
    public class UnitController : MonoBehaviour
    {
        [Header("Spawn Parameters")]
        
        [SerializeField] private float playerDelay;
        [SerializeField] private float obstacleDelay;
        [SerializeField] private float enemyDelay;

        private readonly Dictionary<IUnit, Animator> players = new Dictionary<IUnit, Animator>();
        private readonly Dictionary<IUnit, Animator> enemies = new Dictionary<IUnit, Animator>();
        private readonly Dictionary<GridObject, Animator> obstacles = new Dictionary<GridObject, Animator>();
        
        private CommandManager commandManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private GridManager gridManager;

        private static readonly int spawn = Animator.StringToHash("spawn");
        private static readonly int death = Animator.StringToHash("death");

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            gridManager = ManagerLocator.Get<GridManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<UnitsReadyCommand<PlayerUnitData>>) OnPlayersReady);
            commandManager.ListenCommand((Action<UnitsReadyCommand<EnemyUnitData>>) OnEnemiesReady);
            commandManager.ListenCommand((Action<GridObjectsReadyCommand>) OnGridObjectsReady);
            commandManager.ListenCommand((Action<TurnQueueCreatedCommand>) OnTurnQueueCreated);
            commandManager.ListenCommand((Action<KilledUnitCommand>) OnUnitKilled);
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnEncounterWon);
            commandManager.ListenCommand((Action<NoRemainingPlayerUnitsCommand>) OnEncounterLost);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<UnitsReadyCommand<PlayerUnitData>>) OnPlayersReady);
            commandManager.UnlistenCommand((Action<UnitsReadyCommand<EnemyUnitData>>) OnEnemiesReady);
            commandManager.UnlistenCommand((Action<GridObjectsReadyCommand>) OnGridObjectsReady);
            commandManager.UnlistenCommand((Action<TurnQueueCreatedCommand>) OnTurnQueueCreated);
            commandManager.UnlistenCommand((Action<KilledUnitCommand>) OnUnitKilled);
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnEncounterWon);
            commandManager.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnEncounterLost);
        }

        private void OnPlayersReady(UnitsReadyCommand<PlayerUnitData> cmd)
        {
            foreach (IUnit unit in Shuffle(playerManager.Units.ToArray()))
            {
                Animator animator = unit.transform.parent.GetComponentInChildren<Animator>();
                if (!animator)
                    continue;

                players.Add(unit, animator);
            }
        }
        
        private void OnEnemiesReady(UnitsReadyCommand<EnemyUnitData> cmd)
        {
            foreach (IUnit unit in Shuffle(enemyManager.Units.ToArray()))
            {
                Animator animator = unit.transform.parent.GetComponentInChildren<Animator>();
                if (!animator)
                    continue;

                enemies.Add(unit, animator);
            }
        }

        private void OnGridObjectsReady(GridObjectsReadyCommand cmd)
        {
            GridObject[] gridObjects = gridManager.tileDatas.Values.SelectMany(t => t.GridObjects).
                Where(go => !(go is IUnit)).ToArray();

            foreach (GridObject gridObject in Shuffle(gridObjects))
            {
                Animator animator = gridObject.transform.parent.GetComponentInChildren<Animator>();
                if (!animator)
                    continue;
                
                obstacles.Add(gridObject, animator);
            }
        }

        private void OnTurnQueueCreated(TurnQueueCreatedCommand cmd)
        {
            SpawnUnits(obstacles.Values, obstacleDelay);
            SpawnUnits(enemies.Values, enemyDelay);
            SpawnUnits(players.Values, playerDelay);
        }

        private void OnUnitKilled(KilledUnitCommand cmd)
        {
            players.TryGetValue(cmd.Unit, out Animator animator);
            
            if (!animator)
                enemies.TryGetValue(cmd.Unit, out animator);

            if (!animator)
                return;
            
            animator.SetTrigger(death);
        }

        private static T[] Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i >= 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }

            return array;
        }

        private async void SpawnUnits(IEnumerable<Animator> animators, float delay = 0.0f)
        {
            await UniTask.Delay((int) (delay * 1000.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
            
            foreach (Animator animator in animators)
            {
                if (!Application.isPlaying)
                    return;
                
                animator.SetTrigger(spawn);
            }
        }

        private async void HideUnits(IEnumerable<Animator> animators, float delay = 0.0f)
        {
            await UniTask.Delay((int) (delay * 1000.0f), cancellationToken: this.GetCancellationTokenOnDestroy());
            
            foreach (Animator animator in animators)
            {
                if (!Application.isPlaying)
                    return;
                
                animator.SetTrigger(death);
            }
        }

        private void OnEncounterWon(NoRemainingEnemyUnitsCommand cmd)
        {
            HideUnits(players.Values);
            HideUnits(obstacles.Values);
        }

        private void OnEncounterLost(NoRemainingPlayerUnitsCommand cmd)
        {
            HideUnits(enemies.Values);
            HideUnits(obstacles.Values);
        }
    }
}
