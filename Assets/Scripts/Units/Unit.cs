using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using StatusEffects;
using Abilities;
using Cysharp.Threading.Tasks;
using Managers;
using TMPro;
using Units.Commands;
using UnityEngine;
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
        private SpriteRenderer spriteRenderer;
        
        public string Name
        {
            get => data.Name;
            set => data.Name = value;
        }
        public Health Health { get; private set; }
        public Knockback Knockback { get; private set; }
        
        public Animator UnitAnimator { get; private set; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public Color UnitColor => spriteRenderer.color;
        
        public TenetType Tenet => data.Tenet;

        public ValueStat MovementActionPoints
        {
            get => data.MovementPoints;
            set
            {
                MovementActionPoints = value;
                commandManager.ExecuteCommand(new MovementActionPointChangedCommand(this,value.Value));
            }
        }

        public ValueStat Speed
        {
            get => data.Speed;
            set
            {
                Speed = value;
                commandManager.ExecuteCommand(new SpeedChangedCommand(this, value.Value));
            }
        }
        public ModifierStat Attack => data.Attack;

        public List<Ability> Abilities
        {
            get => data.Abilities;
            set
            {
                Abilities = value;
                commandManager.ExecuteCommand(new AbilitiesChangedCommand(this,value));
            }
        }

        [Obsolete("Use TenetStatuses instead")]
        public ICollection<TenetStatus> TenetStatusEffects => TenetStatuses;
        public ICollection<TenetStatus> TenetStatuses => tenetStatusEffectSlots;

        public static Type DataType => typeof(T);
        
        public Sprite Render => render;
        

        public bool IsSelected => ReferenceEquals(playerManager.SelectedUnit, this);

        private const int maxTenetStatusEffectCount = 2;
        private readonly LinkedList<TenetStatus> tenetStatusEffectSlots =
            new LinkedList<TenetStatus>();

        private AnimationStates unitAnimationState;
        
        private TurnManager turnManager;
        private PlayerManager playerManager;
        private CommandManager commandManager;

        private void Awake() => spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        protected override void Start()
        {
            base.Start();

            data.Initialise();
            Health = new Health(new TakeRawDamageCommand(this,0), new KillUnitCommand(this),
                data.HealthPoints, data.Defence);
            Knockback = new Knockback(data.TakeKnockbackModifier);
            UnitAnimator = GetComponentInChildren<Animator>();

            
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
        
        public void TakeDefence(int amount)
        {
            Health.Defence.Adder -= amount;
            commandManager.ExecuteCommand(new AttackChangeCommand(this,amount));

        }

        public void TakeAttack(int amount)
        {
            Attack.Adder += amount;
            commandManager.ExecuteCommand(new AttackChangeCommand(this,amount));
        } 

        public void TakeDamage(int amount)
        {
            int damageTaken = Health.TakeDamage(amount);
            commandManager.ExecuteCommand(new TakeTotalDamageCommand(this,amount));

        }

        public void TakeKnockback(int amount)
        {
            Knockback.TakeKnockback(amount);
            commandManager.ExecuteCommand(new KnockbackModifierChangedCommand(this,amount));

        } 
        
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
            gridManager.RemoveGridObject(Coordinate, this);
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

        #region AnimationHandling

        public void ChangeAnimation(AnimationStates animationStates) // this stuff is temporary, should probably be done in a better way
        {
            unitAnimationState = animationStates;

            switch (unitAnimationState)
            {
                case AnimationStates.Idle:
                    UnitAnimator.SetBool("moving", false);
                    UnitAnimator.SetBool("front", true);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Down:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", true);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Up:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", false);
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Left:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", true);
                    spriteRenderer.flipX = true;
                    break;
                
                case AnimationStates.Right:
                    UnitAnimator.SetBool("moving", true);
                    UnitAnimator.SetBool("front", false);
                    spriteRenderer.flipX = false;
                    break;
                
                case AnimationStates.Casting:
                    UnitAnimator.SetBool("moving", false);
                    UnitAnimator.SetTrigger("attack");
                    break;
            }
        }

        #endregion
        
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
