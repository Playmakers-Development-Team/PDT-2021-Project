using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using UI;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace Managers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private Transform abilityParent;

        private TurnManager gridManager;
        private UIManager uiManager;
        private CommandManager commandManager;
        private UnitManager unitManager;

        /// <summary>
        /// Stores the current actingunit.
        /// </summary>
        private PlayerUnit actingUnit => (PlayerUnit)unitManager.GetCurrentActingPlayerUnit;

        /// <summary>
        /// A list of ability cards showing the units current abilities
        /// </summary>
        private List<AbilityCard> abilityCards = new List<AbilityCard>();

        /// <summary>
        /// A check to determine if the timeline is ready
        /// </summary>
        private bool timelineIsReady;

        /// <summary>
        /// A check to determine if the ability has been unselected
        /// </summary>
        private bool isUnselected;

        /// <summary>
        /// The current index of the current acting player unit 
        /// </summary>
        private int abilityIndex;

        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            uiManager = ManagerLocator.Get<UIManager>();

            commandManager.ListenExecuteCommand<TurnQueueCreatedCommand>(cmd =>
            {
                timelineIsReady = true;

                if (actingUnit == null)
                    return;

                abilityIndex = 0;
                UpdateAbilityUI((PlayerUnit)actingUnit);
            });
        }

        private void Start()
        {
            commandManager.ListenExecuteCommand<UnitSelectedCommand>(cmd =>
            {
                if (!timelineIsReady)
                    return;

                abilityIndex = 0;
                UpdateAbilityUI((PlayerUnit)actingUnit);
            });

            commandManager.ListenExecuteCommand<StartTurnCommand>(cmd =>
            {
                if (unitManager.GetCurrentActiveUnit is EnemyUnit)
                    ClearAbilityUI();

                uiManager.ClearAbilityHighlight();
            });
        }

        /// <summary>
        /// Updated the ability ui cards for the new active unit
        /// </summary>
        /// <param name="the current active unit"></param>
        private void UpdateAbilityUI(PlayerUnit unit)
        {
            ClearAbilityUI();

            if (unit is null)
            {
                Debug.LogWarning(
                    "GridController.UpdateAbilityUI should not be passed a null value. Use GridController.ClearAbilityUI instead.");
                return;
            }

            foreach (var ability in unit.GetAbilities()) // updated formatting to fit convention
                AddAbilityField(ability);
        }

        /// <summary>
        /// Clears the ability cards used when a player unit is deselected
        /// </summary>
        private void ClearAbilityUI()
        {
            foreach (var abilityCard in abilityCards) // updated formatting to fit convention
                Destroy(abilityCard.gameObject);

            abilityCards.Clear();
        }

        /// <summary>
        /// Instantiates the ability cards
        /// </summary>
        private void AddAbilityField(Ability ability)
        {
            var abilityCardObject = Instantiate(abilityUIPrefab, abilityParent);
            var abilityCard = abilityCardObject.GetComponent<AbilityCard>();
            abilityCard.SetAbility(ability);
            abilityCards.Add(abilityCard);
        }

        private void TestAbilityHighlight(IUnit unit, Ability ability)
        {
            uiManager.HighlightAbility(unit.Coordinate,
                ((OrdinalDirection) UnityEngine.Random.Range(0,
                    Enum.GetValues(typeof(OrdinalDirection)).Length)).ToVector2(), ability);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) // SELECTS THE ABILITY PRESSING E MULTIPLE TIMES WILL GO THROUGH THE ABILITY LIST
            {
                if (actingUnit == null)
                    return;

                if (isUnselected)
                {
                    isUnselected = false;
                    UpdateAbilityUI((PlayerUnit)actingUnit);
                }

                if (abilityIndex >= abilityCards.Count)
                    abilityIndex = 0;
                
                if (abilityIndex != 0)
                    abilityCards[abilityIndex - 1].UnHighlightAbility();
                
                
                abilityCards[abilityIndex].HighlightAbility();
                actingUnit.CurrentlySelectedAbility = abilityCards[abilityIndex].Ability;
                TestAbilityHighlight(actingUnit, actingUnit.CurrentlySelectedAbility);

                abilityIndex++;
            }

            if (Input.GetKeyDown(KeyCode.Q)) //DESELECTS ABILITIES
            {
                if (actingUnit == null)
                    return;

                isUnselected = false;
                actingUnit.CurrentlySelectedAbility = null;
                uiManager.ClearAbilityHighlight();
                abilityIndex = 0;
            }
        }
    }
}
