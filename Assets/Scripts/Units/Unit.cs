using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using StatusEffects;
using Abilities;
using Cysharp.Threading.Tasks;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using Units.Commands;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units
{
    public abstract class Unit<T> : GridObject, IUnit where T : UnitData
    {
        [SerializeField] protected T data;
        
        public TenetType Tenet => data.tenet;
        public ValueStat MovementActionPoints => data.movementActionPoints;
        public ValueStat Speed => data.speed;
        public ModifierStat DealDamageModifier => data.dealDamageModifier;
        public List<Ability> Abilities => data.abilities;
        //public Vector2Int Coordinate { get => ((GridObject)this).Coordinate; set; }

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
            Speed.Value += Random.Range(10, 50);

            turnManager = ManagerLocator.Get<TurnManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();

            commandManager.ListenCommand<KillUnitCommand>(OnKillUnitCommand);
        }

        // TODO: Used for testing, can eventually be removed
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && Random.Range(0,2) == 1)
                TakeDamage(10);
        }
        
        public void TakeDefence(int amount) => DealDamageModifier.Adder -= amount;

        public void TakeAttack(int amount) => Health.TakeDamageModifier.Adder += amount;
        
        public void TakeDamage(int amount)
        {
            int damageTaken = Health.TakeDamage(amount);
            
            SpawnDamageText(damageTaken);
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

        public bool IsActing() => turnManager.CurrentUnit == (IUnit) this;

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
            // We have to explicitly compare types here 
            if (killUnitCommand.Unit.GetType() == GetType())
            {
                // Since we're about to remove the object, stop listening to the command
                commandManager.UnlistenCommand<KillUnitCommand>(OnKillUnitCommand);
                KillUnit();
            }
        }

        private async void KillUnit()
        {
            playerManager.WaitForDeath = true;
            Debug.Log($"Unit Killed: {this.name} : {Coordinate}");
            gridManager.RemoveGridObject(this.Coordinate, this);
            await UniTask.Delay(playerManager.DeathDelay);
            playerManager.WaitForDeath = false;
            Debug.Log($"This unit was cringe and died");

            commandManager.ExecuteCommand(new KillingUnitCommand(this));
            gridManager.RemoveGridObject(Coordinate, this);

            // TODO: This is currently being called twice (see UnitManager.RemoveUnit:110).
            // TODO: This is fixed by the proto-two/integration/unit-death branch.
            // ManagerLocator.Get<TurnManager>().RemoveUnitFromQueue(this);
            ManagerLocator.Get<TurnManager>().RemoveUnitFromQueue(this);

            switch (this)
            {
                case PlayerUnit _:
                    ManagerLocator.Get<PlayerManager>().RemoveUnit(this);
                    break;
                case EnemyUnit _:
                    ManagerLocator.Get<EnemyManager>().RemoveUnit(this);
                    break;
                default:
                    Debug.LogError("ERROR: Failed to kill " + this.gameObject + 
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
    }
}
