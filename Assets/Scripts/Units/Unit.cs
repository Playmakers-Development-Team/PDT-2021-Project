using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using StatusEffects;
using Abilities;
using Cysharp.Threading.Tasks;
using Managers;
using Tiles;
using TMPro;
using Units.Commands;
using UnityEngine;
using Utility;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Canvas damageTextCanvas; // MUST BE ASSIGNED IN PREFAB INSPECTOR
        [SerializeField] private float damageTextLifetime = 1.0f;
        [SerializeField] private Sprite render;

        public string Name
        {
            get => data.Name;
            set => data.Name = value;
        }
        public Health Health { get; private set; }
        public Knockback Knockback { get; private set; }
        public TenetType Tenet => data.Tenet;
        public ValueStat MovementActionPoints => data.MovementPoints;
        public ValueStat Speed => data.Speed;
        public ModifierStat Attack => data.Attack;
        public List<Ability> Abilities => data.Abilities;

        [Obsolete("Use TenetStatuses instead")]
        public ICollection<TenetStatus> TenetStatusEffects => TenetStatuses;
        public ICollection<TenetStatus> TenetStatuses => tenetStatusEffectSlots;

        public static Type DataType => typeof(T);
        
        public Sprite Render => render;

        public bool IsSelected => ReferenceEquals(playerManager.SelectedUnit, this);

        private const int maxTenetStatusEffectCount = 2;
        private readonly LinkedList<TenetStatus> tenetStatusEffectSlots =
            new LinkedList<TenetStatus>();
        
        private TurnManager turnManager;
        private PlayerManager playerManager;
        private CommandManager commandManager;

        protected override void Start()
        {
            base.Start();

            data.Initialise();
            Health = new Health(new KillUnitCommand(this), data.HealthPoints, data.Defence);
            Knockback = new Knockback(data.TakeKnockbackModifier);

            // TODO Speed temporarily random for now until proper turn manipulation is done.
            Speed.Value += Random.Range(0, 101);

            #region GetManagers

            turnManager = ManagerLocator.Get<TurnManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            commandManager = ManagerLocator.Get<CommandManager>();

            #endregion

            #region ListenCommands

            commandManager.ListenCommand<KillUnitCommand>(OnKillUnitCommand);

            #endregion

            if (nameText)
                nameText.text = Name;
            
            if (healthText)
            {
                healthText.text =
                    (Health.HealthPoints.Value + " / " + Health.HealthPoints.BaseValue);
            }
        }

        protected virtual void Update() {}

        #region ValueChanging
        
        public void TakeDefence(int amount) => Health.Defence.Adder -= amount;

        public void TakeAttack(int amount) => Attack.Adder += amount;

        public void TakeDamage(int amount)
        {
            int damageTaken = Health.TakeDamage(amount);
        }

        public void TakeKnockback(int amount) => Knockback.TakeKnockback(amount);
        
        #endregion

        #region TenetStatusEffect

        public void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1)
        {
            TenetStatus status = new TenetStatus(tenetType, stackCount);

            if (status.IsEmpty)
                return;

            // Try to add on top of an existing tenet type
            if (TryGetTenetStatusNode(status.TenetType,
                out LinkedListNode<TenetStatus> foundNode))
            {
                foundNode.Value += status;
            }
            else
            {
                // When we are already utilizing all the slots
                if (TenetStatuses.Count == maxTenetStatusEffectCount)
                {
                    // Remove the oldest status effect to make space for the new status effect
                    tenetStatusEffectSlots.RemoveFirst();
                }

                tenetStatusEffectSlots.AddLast(status);
            }
        }

        public bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue)
        {
            LinkedListNode<TenetStatus> node = tenetStatusEffectSlots.First;

            while (node != null)
            {
                if (node.Value.TenetType == tenetType)
                {
                    node.Value -= amount;

                    if (node.Value.IsEmpty)
                        tenetStatusEffectSlots.Remove(node);
                    return true;
                }

                node = node.Next;
            }

            return false;
        }

        public void ClearAllTenetStatus() => tenetStatusEffectSlots.Clear();

        [Obsolete("Use TryGetTenetStatus instead")]
        public bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus) =>
            TryGetTenetStatusEffect(tenetType, out tenetStatus);

        public bool TryGetTenetStatusEffect(TenetType tenetType,
                                            out TenetStatus tenetStatus)
        {
            bool isFound = TryGetTenetStatusNode(tenetType,
                out LinkedListNode<TenetStatus> foundNode);
            tenetStatus = isFound ? foundNode.Value : default;
            return isFound;
        }

        [Obsolete("Use GetTenetStatus instead")]
        public int GetTenetStatusEffectCount(TenetType tenetType) =>
            GetTenetStatusCount(tenetType);

        public int GetTenetStatusCount(TenetType tenetType)
        {
            return HasTenetStatus(tenetType)
                ? tenetStatusEffectSlots.Where(s => s.TenetType == tenetType).Sum(s => s.StackCount)
                : 0;
        }

        [Obsolete("Use HasTenetStatus instead")]
        public bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1) =>
            HasTenetStatus(tenetType, minimumStackCount);

        public bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1)
        {
            return tenetStatusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);
        }

        private bool TryGetTenetStatusNode(TenetType tenetType,
                                           out LinkedListNode<TenetStatus> foundNode)
        {
            LinkedListNode<TenetStatus> node = tenetStatusEffectSlots.First;

            while (node != null)
            {
                if (node.Value.TenetType == tenetType)
                {
                    foundNode = node;
                    return true;
                }

                node = node.Next;
            }

            foundNode = null;
            return false;
        }
        
        #endregion

        #region UnitDeath

        /// <summary>
        /// Makes it easier to debug with the command debugger window.
        /// </summary>
        private void OnKillUnitCommand(KillUnitCommand killUnitCommand)
        {
            if (!ReferenceEquals(killUnitCommand.Unit, this))
                return;
            
            // Since we're about to remove the object, stop listening to the command
            commandManager.UnlistenCommand<KillUnitCommand>(OnKillUnitCommand);
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

        #region Scene
        
        private void SpawnDamageText(int damageAmount)
        {
            damageTextCanvas.enabled = true;
            
            damageTextCanvas.GetComponentInChildren<TMP_Text>().text =
                damageAmount.ToString();
            
            Invoke("HideDamageText", damageTextLifetime);
        }

        private void HideDamageText() => damageTextCanvas.enabled = false;

        public void SetName() => nameText.text = Name;

        #endregion
        
        /// <summary>
        /// Returns a list of all coordinates that are reachable from a given starting position
        /// within the given range.
        /// </summary>
        /// <param name="startingCoordinate">The coordinate to begin the search from.</param>
        /// <param name="range">The range from the starting tile using manhattan distance.</param>
        /// <returns>A list of the coordinates of reachable tiles.</returns>
        public List<Vector2Int> GetAllReachableTiles()
        {
            Vector2Int startingCoordinate = Coordinate;
            int range = (int) MovementActionPoints.Value;
            
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
            int moveRange = (int)unit.MovementActionPoints.Value;
            Vector2Int startingCoordinate = unit.Coordinate;
            Vector2Int currentCoordinate = startingCoordinate;
            PlayerUnit playerUnit = null;

            if (unit is PlayerUnit)
            {
                playerUnit = (PlayerUnit) unit;
                if (playerUnit.UnitAnimator != null)
                    playerUnit.UnitAnimator.SetInteger("Movement", 1);
            }

            // Check if tile is unoccupied
            if (tileData.GridObjects.Count != 0)
            {
                // TODO: Provide feedback to the player
                Debug.Log("Target tile is occupied.");
                return;
            }

            // Check if tile is in range
            if (!GetAllReachableTiles().Contains(newCoordinate) &&
                unit.GetType() == typeof(PlayerUnit))
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
                if (playerUnit !=
                    null) // this stuff is temporary, should probably be done in a better way
                {
                    if (movePath[i].x > currentCoordinate.x)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Down);
                    else if (movePath[i].y > currentCoordinate.y)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Left);
                    else if (movePath[i].x < currentCoordinate.x)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Up);
                    else if (movePath[i].y < currentCoordinate.y)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Right);
                }

                await gridManager.MovementTween(unit.gameObject, gridManager.ConvertCoordinateToPosition(currentCoordinate),
                    gridManager.ConvertCoordinateToPosition(movePath[i]), 1f);
                unit.gameObject.transform.position = gridManager.ConvertCoordinateToPosition(movePath[i]);
                currentCoordinate = movePath[i];
            }
            
            gridManager.MoveGridObject(startingCoordinate, newCoordinate, (GridObject) unit);
            unit.MovementActionPoints.Value -= Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate));

            if (playerUnit != null)
                playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Idle);

            /*Debug.Log(Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate)));*/
            
            // Should be called when all the movement and tweening has been completed
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new EndMoveCommand(moveCommand));
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
            
            int randomIndex = UnityEngine.Random.Range(0, names.Length - 1);
            return names[randomIndex];
        }
        
        #endregion
    }
}
