using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities;
using Abilities.Commands;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using Grid.Tiles;
using Managers;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using Units.Stats;
using TenetStatuses;
using Units.Virtual;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;

        private SpriteRenderer spriteRenderer;
        
        public string Name
        {
            get => data.Name;
            set => data.Name = value;
        }
        
        public HealthStat HealthStat { get; private set; }
        public Stat AttackStat { get; private set; }
        public Stat DefenceStat { get; private set; }
        public Stat MovementPoints { get; private set; }
        public Stat SpeedStat { get; private set; }
        public Stat KnockbackStat { get; private set; }

        public bool Indestructible { get; set; }

        public Animator UnitAnimator { get; private set; }
        public Color UnitColor => spriteRenderer.color;
        
        public TenetType Tenet => data.Tenet;

        public List<Ability> Abilities
        {
            get => data.Abilities;
            set
            {
                data.Abilities = value;
                commandManager.ExecuteCommand(new AbilitiesChangedCommand(this, value));
            }
        }
        
        public static Type DataType => typeof(T);
        
        private AnimationStates unitAnimationState;
        
        private PlayerManager playerManager;

        protected UnitManager<T> unitManagerT; 
        
        // TODO: Rename
        private static readonly int movingAnimationParameter = Animator.StringToHash("moving");
        private static readonly int frontAnimationParameter = Animator.StringToHash("front");
        private static readonly int attackAnimationParameter = Animator.StringToHash("attack");

        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            playerManager = ManagerLocator.Get<PlayerManager>();
            
            #endregion
            
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            HealthStat = new HealthStat(KillUnit,this,data.HealthValue.BaseValue, 
            StatTypes.Health);
            DefenceStat = new Stat(this, data.DefenceStat.BaseValue, StatTypes.Defence);
            AttackStat = new Stat(this, data.AttackStat.BaseValue, StatTypes.Attack);
            SpeedStat = new Stat(this, Random.Range(0,101), StatTypes.Speed);
            MovementPoints = new Stat(this, data.MovementPoints.BaseValue, StatTypes.MovementPoints);
            KnockbackStat = new Stat(this, data.KnockbackStat.BaseValue, StatTypes.Knockback);
            TenetStatusEffectsContainer.Initialise(data.StartingTenets);

            UnitAnimator = GetComponentInChildren<Animator>();
        }

        #region ValueChanging
        
        public void TakeDefence(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeDefence(amount);
            virtualUnit.ApplyChanges();
        }

        public void TakeAttack(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeAttack(amount);
            virtualUnit.ApplyChanges();
        }

        public void TakeAttackForEncounter(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeAttackForEncounter(amount);
            virtualUnit.ApplyChanges();
        }

        public void TakeDefenceForEncounter(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeDefenceForEncounter(amount);
            virtualUnit.ApplyChanges();
        }

        public void TakeDamage(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeDamage(amount);
            virtualUnit.ApplyChanges();
        }

        public void DealDamageTo(IAbilityUser other, int amount)
        {
            IVirtualAbilityUser to = other.CreateVirtualAbilityUser();
            IVirtualAbilityUser from = CreateVirtualAbilityUser();

            from.DealDamageTo(to, amount);
            from.ApplyChanges();
            to.ApplyChanges();
        }

        public void TakeKnockback(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeKnockback(amount);
            virtualUnit.ApplyChanges();
        }

        public void SetSpeed(int amount) => SpeedStat.Value = amount;
        
        public void AddSpeed(int amount)
        {
            IVirtualAbilityUser virtualUnit = CreateVirtualAbilityUser();
            virtualUnit.TakeKnockback(amount);
            virtualUnit.ApplyChanges();
        }

        public IVirtualAbilityUser CreateVirtualAbilityUser() => new VirtualUnit(this);
        
        #endregion
        
        #region UnitDeath

        /// <summary>
        /// Makes it easier to debug with the command debugger window.
        /// </summary>
        private void OnKillUnitCommand(KillUnitCommand killUnitCommand)
        {
            if (!ReferenceEquals(killUnitCommand.Unit, this))
                return;
            
            if (Indestructible) return;

            KillUnit();
        }

        private async void KillUnit()
        {
            playerManager.WaitForDeath = true;
            Debug.Log($"Unit Killed: {name} : {Coordinate}");
            await UniTask.Delay(playerManager.DeathDelay);
            playerManager.WaitForDeath = false;

            commandManager.ExecuteCommand(new KillingUnitCommand(this));
            gridManager.RemoveGridObject(Coordinate, this);

            switch (this)
            {
                case PlayerUnit _:
                    ManagerLocator.Get<PlayerManager>().RemoveUnit(this);
                    break;
                case EnemyUnit _:
                    ManagerLocator.Get<EnemyManager>().RemoveUnit(this);
                    break;
                default:
                    Debug.LogError("ERROR: Failed to kill " + gameObject +
                                   " as it is an unidentified unit");
                    break;
            }

            // "Delete" the gridObject (setting it to inactive just in case we still need it)
            gameObject.SetActive(false);
            
            commandManager.ExecuteCommand(new KilledUnitCommand(this));
        }
        
        #endregion

        #region AnimationHandling

        public async void ChangeAnimation(AnimationStates animationStates) 
        {
            unitAnimationState = animationStates;

            switch (unitAnimationState)
            {
                case AnimationStates.Idle:
                    UnitAnimator.SetBool(movingAnimationParameter, false);
                    UnitAnimator.SetBool(frontAnimationParameter, true);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Down:
                    UnitAnimator.SetBool(movingAnimationParameter, true);
                    UnitAnimator.SetBool(frontAnimationParameter, true);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Up:
                    UnitAnimator.SetBool(movingAnimationParameter, true);
                    UnitAnimator.SetBool(frontAnimationParameter, false);
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Left:
                    UnitAnimator.SetBool(movingAnimationParameter, true);
                    UnitAnimator.SetBool(frontAnimationParameter, true);
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Right:
                    UnitAnimator.SetBool(movingAnimationParameter, true);
                    UnitAnimator.SetBool(frontAnimationParameter, false);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Casting:
                    UnitAnimator.SetBool(movingAnimationParameter, false);
                    UnitAnimator.SetTrigger(attackAnimationParameter);

                    await Task.Delay((int) UnitAnimator.GetCurrentAnimatorStateInfo(0).length * 1000);

                    commandManager.ExecuteCommand(new EndUnitCastingCommand(this));
                    break;
            }
        }

        #endregion

        #region Utility

        public abstract bool IsSameTeamWith(IAbilityUser other);

        #endregion
        
        /// <summary>
        /// Returns a list of all coordinates that are reachable from a given starting position
        /// within the given range.
        /// </summary>
        /// <returns>A list of the coordinates of reachable tiles.</returns>
        public List<Vector2Int> GetAllReachableTiles()
        {
            Vector2Int startingCoordinate = Coordinate;
            int range = MovementPoints.Value;
            
            List<Vector2Int> reachable = new List<Vector2Int>();
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            string allegiance = "";

            if (gridManager.tileDatas[startingCoordinate].GridObjects.Count > 0)
            {
                allegiance= gridManager.tileDatas[startingCoordinate].GridObjects[0].tag;
            }
            
            // Add the starting coordinate to the queue
            coordinateQueue.Enqueue(startingCoordinate);
            int distance = 0;
            visited.Add(startingCoordinate, distance);

            // Loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];

                if (distance > range)
                {
                    break;
                }

                // Add neighbours of node to queue
                Pathfinding.VisitNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                Pathfinding.VisitNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                Pathfinding.VisitNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                Pathfinding.VisitNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);

                if (gridManager.GetGridObjectsByCoordinate(currentNode).Count == 0)
                    reachable.Add(currentNode);

                coordinateQueue.Dequeue();
            }

            return reachable;
        }

        public async void MoveUnit(StartMoveCommand moveCommand)
        {
            IUnit unit = this;
            Vector2Int newCoordinate = moveCommand.TargetCoords;

            TileData tileData = gridManager.GetTileDataByCoordinate(newCoordinate);
            
            if (tileData is null)
            {
                throw new Exception($"No tile data at coordinate {newCoordinate}. " +
                                    "Failed to move unit");
            }
            
            Vector2Int startingCoordinate = unit.Coordinate;
            Vector2Int currentCoordinate = startingCoordinate;

            // Check if tile is unoccupied
            if (tileData.GridObjects.Count != 0)
            {
                // TODO: Provide feedback to the player
                Debug.Log("Target tile is occupied.");
                return;
            }

            // Check if tile is in range
            if (!GetAllReachableTiles().Contains(newCoordinate) 
                && unit.GetType() == typeof(PlayerUnit))
            {
                // TODO: Provide feedback to the player
                Debug.Log("MANHATTTAN STUFF OUT OF RANGE" +
                          ManhattanDistance.GetManhattanDistance(startingCoordinate,
                              newCoordinate));

                Debug.Log("Target tile out of range.");
                return;
            }

            // TODO: Tween based on cell path
            List<Vector2Int> movePath = Pathfinding.GetCellPath(currentCoordinate, newCoordinate, unit);

            for (int i = 1; i < movePath.Count; i++)
            {
                unit.UnitAnimator.SetBool("moving", true);
            
                if (movePath[i].x > currentCoordinate.x)
                    unit.ChangeAnimation(AnimationStates.Right);
                else if (movePath[i].y > currentCoordinate.y)
                    unit.ChangeAnimation(AnimationStates.Up);
                else if (movePath[i].x < currentCoordinate.x)
                    unit.ChangeAnimation(AnimationStates.Left);
                else if (movePath[i].y < currentCoordinate.y)
                    unit.ChangeAnimation(AnimationStates.Down);

                await gridManager.MovementTween(unit.gameObject, 
                    gridManager.ConvertCoordinateToPosition(currentCoordinate),
                    gridManager.ConvertCoordinateToPosition(movePath[i]), 1f);
                unit.gameObject.transform.position =
                    gridManager.ConvertCoordinateToPosition(movePath[i]);
                currentCoordinate = movePath[i];
            }
            
            int manhattanDistance = Mathf.Max(0, ManhattanDistance.GetManhattanDistance(
                startingCoordinate,
                newCoordinate
            ));
            
            gridManager.MoveGridObject(startingCoordinate, newCoordinate, (GridObject) unit);
            unit.MovementPoints.Value -= manhattanDistance;
            unit.ChangeAnimation(AnimationStates.Idle);

            /*Debug.Log(Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate)));*/
            
            // Should be called when all the movement and tweening has been completed
            commandManager.ExecuteCommand(new EndMoveCommand(moveCommand));
        }

        #region RandomizeNames
        
        public string RandomizeName()
        {
            string[] names =
            {
                "Agid", "Jack", "Francisco", "Kyle", "Jordan", "Sam", "Jake", "William",
                "Beatrice", "Lachlan", "Hugo", "Habib", "Christa", "Roy", "Nick", "Eddie",
                "Vivian", "Ethan", "Jaiden", "Jamie", "Leon", "Groovy Bot", "Clickup Bot",
                "Github-Bot"
            };
            
            int randomIndex = Random.Range(0, names.Length - 1);
            return names[randomIndex];
        }
        
        #endregion
        
        // TODO: Add to correct region
        protected void Spawn() => unitManagerT.Spawn(this);
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            commandManager.ListenCommand<KillUnitCommand>(OnKillUnitCommand);
            commandManager.ListenCommand<AbilityCommand>(OnAbility);
            commandManager.ListenCommand<UnitManagerReadyCommand<T>>(OnUnitManagerReady);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            commandManager.UnlistenCommand<KillUnitCommand>(OnKillUnitCommand);
            commandManager.UnlistenCommand<AbilityCommand>(OnAbility);
            commandManager.UnlistenCommand<UnitManagerReadyCommand<T>>(OnUnitManagerReady);
        }

        private void OnAbility(AbilityCommand cmd)
        {
            if (!ReferenceEquals(cmd.AbilityUser, this))
                return;

            ChangeAnimation(AnimationStates.Casting);
        }

        private void OnUnitManagerReady(UnitManagerReadyCommand<T> cmd) => Spawn();
        
        #region TenetStatusEffects

        private TenetStatusEffectsContainer TenetStatusEffectsContainer { get; } = new TenetStatusEffectsContainer();

        public ICollection<TenetStatus> TenetStatuses =>
            TenetStatusEffectsContainer.TenetStatuses;

        public void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1) =>
            TenetStatusEffectsContainer.AddOrReplaceTenetStatus(tenetType, stackCount);

        public bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue) =>
            TenetStatusEffectsContainer.RemoveTenetStatus(tenetType, amount);

        public void ClearAllTenetStatus() =>
            TenetStatusEffectsContainer.ClearAllTenetStatus();

        public int GetTenetStatusCount(TenetType tenetType) =>
            TenetStatusEffectsContainer.GetTenetStatusCount(tenetType);

        public bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1) =>
            TenetStatusEffectsContainer.HasTenetStatus(tenetType, minimumStackCount);
        
        public bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus) =>
            TenetStatusEffectsContainer.TryGetTenetStatus(tenetType, out tenetStatus);

        public void SetTenets(ITenetBearer tenetBearer) =>
            TenetStatusEffectsContainer.SetTenets(tenetBearer);

        #endregion
    }
}
