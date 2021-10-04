using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using Grid;
using Grid.Commands;
using Grid.GridObjects;
using Managers;
using Turn;
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
        
        [SerializeField] private float spawnDelay;
        [SerializeField] private float typeDelay;
        [SerializeField] private float spawnPeriod;

        private readonly Dictionary<IUnit, Animator> players = new Dictionary<IUnit, Animator>();
        private readonly Dictionary<IUnit, Animator> enemies = new Dictionary<IUnit, Animator>();
        private readonly Dictionary<GridObject, Animator> obstacles = new Dictionary<GridObject, Animator>();
        
        private CommandManager commandManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private GridManager gridManager;
        private TurnManager turnManager;
        
        private static readonly int spawn = Animator.StringToHash("spawn");

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<UnitsReadyCommand<PlayerUnitData>>) OnPlayersReady);
            commandManager.ListenCommand((Action<UnitsReadyCommand<EnemyUnitData>>) OnEnemiesReady);
            commandManager.ListenCommand((Action<GridObjectsReadyCommand>) OnGridObjectsReady);
            commandManager.ListenCommand((Action<TurnQueueCreatedCommand>) OnTurnQueueCreated);
            commandManager.ListenCommand((Action<KilledUnitCommand>) OnUnitKilled);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<UnitsReadyCommand<PlayerUnitData>>) OnPlayersReady);
            commandManager.UnlistenCommand((Action<UnitsReadyCommand<EnemyUnitData>>) OnEnemiesReady);
            commandManager.UnlistenCommand((Action<GridObjectsReadyCommand>) OnGridObjectsReady);
            commandManager.UnlistenCommand((Action<TurnQueueCreatedCommand>) OnTurnQueueCreated);
            commandManager.UnlistenCommand((Action<KilledUnitCommand>) OnUnitKilled);
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

        private async void OnTurnQueueCreated(TurnQueueCreatedCommand cmd)
        {
            await Task.Delay((int) (spawnDelay * 1000.0f));
            await SpawnUnits(obstacles.Values);
            
            await Task.Delay((int) (typeDelay * 1000.0f));
            await SpawnUnits(enemies.Values);
            
            await Task.Delay((int) (typeDelay * 1000.0f));
            await SpawnUnits(players.Values);
        }

        private void OnUnitKilled(KilledUnitCommand cmd)
        {
            /*if (!unitAnimators.ContainsKey(cmd.Unit))
                return;
            
            unitAnimators.Add*/
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

        private async Task SpawnUnits(IEnumerable<Animator> animators)
        {
            foreach (Animator animator in animators)
            {
                if (!Application.isPlaying)
                    return;
                
                animator.SetTrigger(spawn);
                // await Task.Delay((int) (spawnPeriod * 1000.0f));
            }
        }
    }
}
