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
using Random = UnityEngine.Random;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;

        public string Name
        {
            get => data.name;
            set => data.name = value;
        }

        public TenetType Tenet => data.tenet;
        public ValueStat MovementActionPoints => data.movementActionPoints;
        public ValueStat Speed => data.speed;
        public ModifierStat Attack => data.dealDamageModifier;
        public List<Ability> Abilities => data.abilities;

        public static Type DataType => typeof(T);

        public Type GetDataType() => DataType;

        public int TenetStatusEffectCount => tenetStatusEffectSlots.Count;

        public IEnumerable<TenetStatusEffect> TenetStatusEffects =>
            tenetStatusEffectSlots.AsEnumerable();

        private readonly LinkedList<TenetStatusEffect> tenetStatusEffectSlots =
            new LinkedList<TenetStatusEffect>();

        private const int maxTenetStatusEffectCount = 2;

        public Health Health { get; private set; }
        public Knockback Knockback { get; private set; }

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Canvas damageTextCanvas; // MUST BE ASSIGNED IN PREFAB INSPECTOR
        [SerializeField] private float damageTextLifetime = 1.0f;

        private TurnManager turnManager;
        private PlayerManager playerManager;
        private GridManager gridManager;
        private CommandManager commandManager;

        protected override void Start()
        {
            base.Start();

            data.Initialise();
            Health = new Health(new KillUnitCommand(this), data.healthPoints, data.takeDamageModifier);
            
            // TODO Are speeds are random or defined in UnitData?
            Speed.Value += Random.Range(0, 101);

            turnManager = ManagerLocator.Get<TurnManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.ListenCommand<KillUnitCommand>(OnKillUnitCommand);
            
            if (nameText)
                nameText.text = Name;
            
               
            if (healthText)
                healthText.text = (Health.HealthPoints.Value + " / " + Health.HealthPoints.BaseValue);
        }

        // TODO: Used for testing, can eventually be removed
        private void Update()
        {

        }
        
        
        
        public void TakeDefence(int amount) => Health.Defence.Adder -= amount;

        public void TakeAttack(int amount) => Attack.Adder += amount;
        
        public void TakeDamage(int amount)
        {
            int damageTaken = Health.TakeDamage(amount);
            
            SpawnDamageText(damageTaken);
            
            if (healthText)
                healthText.text = (Health.HealthPoints.Value + " / " + Health.HealthPoints.BaseValue);
            
        }

        public void TakeKnockback(int amount) => Knockback.TakeKnockback(amount);

        public void AddOrReplaceTenetStatusEffect(TenetType tenetType, int stackCount = 1)
        {
            TenetStatusEffect statusEffect = new TenetStatusEffect(tenetType, stackCount);

            if (statusEffect.IsEmpty)
                return;

            // Try to add on top of an existing tenet type
            if (TryGetTenetStatusEffectNode(statusEffect.TenetType,
                out LinkedListNode<TenetStatusEffect> foundNode))
            {
                foundNode.Value += statusEffect;
            }
            else
            {
                // When we are already utilizing all the slots
                if (TenetStatusEffectCount == maxTenetStatusEffectCount)
                {
                    // Remove the oldest status effect to make space for the new status effect
                    tenetStatusEffectSlots.RemoveFirst();
                }

                tenetStatusEffectSlots.AddLast(statusEffect);
            }
        }

        public bool RemoveTenetStatusEffect(TenetType tenetType, int amount = int.MaxValue)
        {
            LinkedListNode<TenetStatusEffect> node = tenetStatusEffectSlots.First;

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

        public void ClearAllTenetStatusEffects() => tenetStatusEffectSlots.Clear(); // just saw this and changed it to fit our style
        
        public bool TryGetTenetStatusEffect(TenetType tenetType,
                                            out TenetStatusEffect tenetStatusEffect)
        {
            bool isFound = TryGetTenetStatusEffectNode(tenetType,
                out LinkedListNode<TenetStatusEffect> foundNode);
            tenetStatusEffect = isFound ? foundNode.Value : default;
            return isFound;
        }

        public int GetTenetStatusEffectCount(TenetType tenetType)
        {
            return HasTenetStatusEffect(tenetType)
                ? tenetStatusEffectSlots.Where(s => s.TenetType == tenetType).Sum(s => s.StackCount)
                : 0;
        }

        public bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1)
        {
            return tenetStatusEffectSlots.Any(s =>
                s.TenetType == tenetType && s.StackCount >= minimumStackCount);
        }

        public bool IsSelected() => playerManager.SelectedUnit == (IUnit) this;

        private bool TryGetTenetStatusEffectNode(TenetType tenetType,
                                                 out LinkedListNode<TenetStatusEffect> foundNode)
        {
            LinkedListNode<TenetStatusEffect> node = tenetStatusEffectSlots.First;

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

        /// <summary>
        /// Makes it easier to debug with the command debugger window.
        /// </summary>
        private void OnKillUnitCommand(KillUnitCommand killUnitCommand)
        {
            if (killUnitCommand.Unit == this)
            {
                // Since we're about to remove the object, stop listening to the command
                commandManager.UnlistenCommand<KillUnitCommand>(OnKillUnitCommand);
                KillUnit();
            }
        }

        private async void KillUnit()
        {
            playerManager.WaitForDeath = true;
            Debug.Log($"Unit Killed: {name} : {Coordinate}");
            await UniTask.Delay(playerManager.DeathDelay);
            playerManager.WaitForDeath = false;
            Debug.Log($"This unit was cringe and died");

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

        private void SpawnDamageText(int damageAmount)
        {
            damageTextCanvas.enabled = true;
            
            damageTextCanvas.GetComponentInChildren<TMP_Text>().text =
                damageAmount.ToString();
            
            Invoke("HideDamageText", damageTextLifetime);
        }

        private void HideDamageText()
        {
            damageTextCanvas.enabled = false;
        }
        
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
                gridManager.VisitNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                gridManager.VisitNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                gridManager.VisitNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance,
                    coordinateQueue, allegiance);
                gridManager.VisitNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance,
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
                if (playerUnit.animator != null)
                    playerUnit.animator.SetInteger("Movement", 1);
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
            List<Vector2Int> movePath = gridManager.GetCellPath(currentCoordinate, newCoordinate);

            for (int i = 1; i < movePath.Count; i++)
            {
                if (playerUnit !=
                    null) // this stuff is temporary, should probably be done in a better way
                {
                    if (movePath[i].x > currentCoordinate.x)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Backward);
                    else if (movePath[i].y > currentCoordinate.y)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Left);
                    else if (movePath[i].x < currentCoordinate.x)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Forward);
                    else if (movePath[i].y < currentCoordinate.y)
                        playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Right);
                }

                await gridManager.MovementTween(unit.gameObject, gridManager.ConvertCoordinateToPosition(currentCoordinate),
                    gridManager.ConvertCoordinateToPosition(movePath[i]), 1f);
                unit.gameObject.transform.position = gridManager.ConvertCoordinateToPosition(movePath[i]);
                currentCoordinate = movePath[i];
            }

            gridManager.MoveGridObject(newCoordinate, (GridObject) unit);
            unit.MovementActionPoints.Value -= Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate));

            if (playerUnit != null)
                playerUnit.ChangeAnimation(PlayerUnit.AnimationStates.Idle);

            Debug.Log(Mathf.Max(0,
                ManhattanDistance.GetManhattanDistance(startingCoordinate, newCoordinate)));
            
            // Should be called when all the movement and tweening has been completed
            ManagerLocator.Get<CommandManager>().ExecuteCommand(new EndMoveCommand(moveCommand));
        }

        #region RandomizeNames
        public string RandomizeName()
        {
            string newname = "";
            int random = UnityEngine.Random.Range(1,25);

            switch (random)
            {
                case 1:
                    newname="Agid";
                    break;
                case 2:
                    newname="Jack";
                    break;
                case 3 :
                    newname="Francisco";
                    break;
                case 4:
                    newname="Kyle";
                    break;
                case 5:
                    newname="Jordan";
                    break;
                case 6:
                    newname="Sam";
                    break;
                case 7:
                    newname="Jake";
                    break;
                case 8:
                    newname="William";
                    break;
                case 9:
                    newname="Beatrice";
                    break;
                case 10:
                    newname="Lachlan";
                    break;
                case 11:
                    newname="Hugo";
                    break;
                case 12:
                    newname="Habib";
                    break;
                case 13:
                    newname="Christa";
                    break;
                case 14:
                    newname="Roy";
                    break;
                case 15:
                    newname="Nick";
                    break;
                case 16:
                    newname="Eddie";
                    break;
                case 17:
                    newname="Vivian";
                    break;
                case 18:
                    newname="Ethan";
                    break;
                case 19:
                    newname="Jaiden";
                    break;
                case 20:
                    newname="Jaime";
                    break;
                case 21:
                    newname="Leon";
                    break;
                case 22:
                    newname="Groovy Bot";
                    break;
                case 23:
                    newname="Clickup Bot";
                    break;
                case 24:
                    newname = "Github-Bot";
                    break;
            }
            return newname;
        }
        
        #endregion
        
        
    }
}
