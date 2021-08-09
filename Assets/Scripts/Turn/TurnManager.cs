using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Managers;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using Units.Stats;
using UnityEngine;

namespace Turn
{
    public class TurnManager : Manager
    {
        public enum TurnPhases
        {
            TurnManipulation,
            Movement,
            Ability
        };

        #region Properties and Fields

        public int TotalTurnCount { get; private set; }
        public int RoundCount { get; private set; }
        public int CurrentTurnIndex { get; private set; }
        public int TurnManipulationPhaseIndex { get; private set; }
        public int MovementPhaseIndex { get; private set; }
        public int AbilityPhaseIndex { get; private set; }
        public int PhaseIndex { get; set; }

        public Stat Insight { get; set; }
        
        /// <summary>
        /// The unit that is currently taking its turn. Returns null if no unit is taking its turn.
        /// </summary>
        public IUnit ActingUnit
        {
            get
            {
                if (currentTurnQueue.Count == 0)
                {
                    Debug.LogWarning($"{nameof(ActingUnit)} could not be accessed. " +
                                     $"{nameof(currentTurnQueue)} is empty.");
                    return null;
                }
                
                if (CurrentTurnIndex < 0 || CurrentTurnIndex >= currentTurnQueue.Count)
                {
                    Debug.LogWarning($"{nameof(ActingUnit)} could not be accessed. " +
                                     $"{nameof(CurrentTurnIndex)} is not valid.");
                    return null;
                }
                
                return currentTurnQueue[CurrentTurnIndex];
            }
        }

        public IUnit PreviousActingUnit => CurrentTurnIndex == 0 ? null : currentTurnQueue[CurrentTurnIndex - 1];

        public IReadOnlyList<IUnit> CurrentTurnQueue => currentTurnQueue.AsReadOnly();
        public IReadOnlyList<IUnit> NextTurnQueue => nextTurnQueue.AsReadOnly();
        public IReadOnlyList<IUnit> PreviousTurnQueue => previousTurnQueue.AsReadOnly();
        public PlayerUnit ActingPlayerUnit => GetActingPlayerUnit();
        public EnemyUnit ActingEnemyUnit => GetActingEnemyUnit();

        private CommandManager commandManager;
        private UnitManager unitManager;

        private readonly List<IUnit> preMadeTurnQueue = new List<IUnit>();
        private List<IUnit> previousTurnQueue = new List<IUnit>();
        private List<IUnit> currentTurnQueue = new List<IUnit>();
        private List<IUnit> nextTurnQueue = new List<IUnit>();
        
        private List<IUnit> unitsTurnManipulatedThisRound = new List<IUnit>();
        private List<IUnit> unitsMeditatedThisRound = new List<IUnit>();
        private List<IUnit> unitsMeditatedLastRound = new List<IUnit>();

        private bool randomizedSpeed = true;

        #endregion

        #region Manager Overrides

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();

            commandManager.ListenCommand<EndTurnCommand>(cmd => NextTurn());
            commandManager.ListenCommand<SpawnedUnitCommand>(cmd => UpdateNextTurnQueue());
            commandManager.ListenCommand<KilledUnitCommand>(cmd => RemoveUnitFromQueue(cmd.Unit));
            commandManager.ListenCommand<EndMoveCommand>(cmd => {
                // TODO: Will be the same for enemy units once they start using abilities
                if (cmd.Unit is PlayerUnit)
                    EndMovementPhase();
            });

            commandManager.ListenCommand<EndUnitCastingCommand>(cmd => {
                // TODO: Will be the same for enemy units once they start using abilities
                if (cmd.Unit is PlayerUnit)
                    EndAbilityPhase();
            });

            commandManager.ListenCommand<EnemyActionsCompletedCommand>(cmd =>
                commandManager.ExecuteCommand(new EndTurnCommand(cmd.Unit)));
        }

        #endregion

        #region Turn Queue Manipulation

        /// <summary>
        /// Create a turn queue based on existing player and enemy units.
        /// Should be called after the level is loaded and all the units are ready.
        /// </summary>
        public void SetupTurnQueue(TurnPhases[] newTurnPhases)
        {
            SetupTurnPhases(newTurnPhases);
            
            RoundCount = 0;
            TotalTurnCount = 0;
            CurrentTurnIndex = 0;
            Insight = new Stat(null, 0, StatTypes.Insight);

            previousTurnQueue = new List<IUnit>();
            currentTurnQueue = CreateTurnQueue();
            UpdateNextTurnQueue();

            commandManager.ExecuteCommand(new TurnQueueCreatedCommand());
            StartTurn();
        }

        public void SetupTurnQueue(GameObject[] premadeTimeline, TurnPhases[] newTurnPhases )
        {
            randomizedSpeed = false;

            foreach(GameObject prefab in premadeTimeline)
                preMadeTurnQueue.Add(prefab.GetComponent<IUnit>());

            SetupTurnQueue(newTurnPhases);
        }

        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <exception cref="IndexOutOfRangeException">If the unit is not in the turn queue.</exception>
        private void RemoveUnitFromQueue(IUnit unit) =>
            RemoveUnitFromQueue(FindTurnIndexFromCurrentQueue(unit));

        /// <summary>
        /// Finish the current turn and end the round if this is the last turn.
        /// </summary>
        private void NextTurn()
        {
            UpdateNextTurnQueue();
            CurrentTurnIndex++;
            TotalTurnCount++;

            if (!CheckUnitsRemaining())
                return;

            if (CurrentTurnIndex >= currentTurnQueue.Count)
                NextRound();

            StartTurn();
        }

        private void StartTurn()
        {
            commandManager.ExecuteCommand(new StartTurnCommand(ActingUnit));

            PhaseIndex = 0;
        }

        /// <summary>
        /// Finish the current round. May transition to the next round or finish the encounter if
        /// there are no enemy or player units remaining.
        /// </summary>
        private void NextRound()
        {
            RoundCount++;
            commandManager.ExecuteCommand(new PrepareRoundCommand());

            previousTurnQueue = new List<IUnit>(currentTurnQueue);
            currentTurnQueue = CreateTurnQueue();
            UpdateNextTurnQueue();

            CurrentTurnIndex = 0;

            unitsMeditatedLastRound = unitsMeditatedThisRound.ToList();
            unitsMeditatedThisRound.Clear();
            unitsTurnManipulatedThisRound.Clear();
            
            ResetUnitStatsAfterRound();
            commandManager.ExecuteCommand(new StartRoundCommand());
        }

        private bool CheckUnitsRemaining()
        {
            // TODO Add option for a draw
            if (!HasEnemyUnitInQueue())
            {
                // Sets the audio to out of combat version. TODO Move this to the GameManager or MusicManager
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");

                commandManager.ExecuteCommand(new NoRemainingEnemyUnitsCommand());

                return false;
            }

            if (!HasPlayerUnitInQueue())
            {
                // Sets the audio to out of combat version. TODO Move this to the GameManager or MusicManager
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");

                commandManager.ExecuteCommand(new NoRemainingPlayerUnitsCommand());

                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Create a turn queue from every available <c>Unit</c> in <c>PlayerManager</c> and
        /// <c>EnemyManager</c>. Calculate the turn order based on the parameters.
        /// </summary>
        private List<IUnit> CreateTurnQueue()
        {
            if (!randomizedSpeed)
            {
                if (preMadeTurnQueue.Count >= unitManager.AllUnits.Count)
                    return preMadeTurnQueue;
                
                Debug.LogWarning("Premade queue was not completed. Switching to speed order." +
                                 $"Expected {unitManager.AllUnits.Count} units, found {preMadeTurnQueue.Count}.");
                randomizedSpeed = true;
            }

            List<IUnit> turnQueue = new List<IUnit>();
            turnQueue.AddRange(unitManager.AllUnits);
            
            // Sort units by speed in descending order
            turnQueue.Sort((x, y) => y.SpeedStat.Value.CompareTo(x.SpeedStat.Value));
            return turnQueue;
        }

        /// <summary>
        /// Should be called whenever the number of units in the turn queue has been changed.
        /// </summary>
        private void UpdateNextTurnQueue() => nextTurnQueue = CreateTurnQueue();

        /// <summary>
        /// Adds a unit to the end of the current turn queue.
        /// </summary>
        [Obsolete]
        public void AddNewUnitToTimeline(IUnit unit)
        {
            if (!currentTurnQueue.Contains(unit)) currentTurnQueue.Add(unit);
        }

        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="targetIndex">The index of the unit to remove.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        private void RemoveUnitFromQueue(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not remove unit at index {targetIndex}");

            // BUG: Removing the first unit on its turn will skip the second unit's turn.
            if (targetIndex <= CurrentTurnIndex && PreviousActingUnit != null)
                CurrentTurnIndex--;

            currentTurnQueue.RemoveAt(targetIndex);
            UpdateNextTurnQueue();

            // If the ActingUnit was removed, start the next unit's turn
            if (targetIndex - 1 == CurrentTurnIndex)
                NextTurn();
        }

        #endregion

        #region Turn Manipulation

        /// <summary>
        /// Find the turn index of a unit of the current turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromCurrentQueue(IUnit unit) => currentTurnQueue.FindIndex(u => u == unit);

        /// <summary>
        /// Find the turn index of a unit of the previous turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromPreviousQueue(IUnit unit) => previousTurnQueue.FindIndex(u => u == unit);

        /// <summary>
        /// Find the turn index of a unit of the next turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromNextQueue(IUnit unit) => nextTurnQueue.FindIndex(u => u == unit);
        
        public void Meditate()
        {
            if (!UnitCanMeditate(ActingUnit))
            {
                Debug.LogWarning($"{ActingUnit} cannot meditate.");
                return;
            }
            
            commandManager.ExecuteCommand(new MeditatedCommand(ActingUnit));
            unitsMeditatedThisRound.Add(ActingUnit);
            Insight.Value += 1;
            EndTurnManipulationPhase();
        }

        /// <summary>
        /// Move a unit right before the current unit. The moved unit will take a turn instantly
        /// before continuing to the current unit.
        /// </summary>
        /// <param name="targetIndex">The index of the unit to be moved.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void MoveTargetBeforeCurrent(int targetIndex)
        {
            ShiftTurnQueue(CurrentTurnIndex, targetIndex);
            StartTurn();
        }

        /// <summary>
        /// Move a unit right after the current unit. The moved unit will take a turn after the
        /// current unit.
        /// </summary>
        /// <param name="targetIndex">The index of the unit to be moved.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void MoveTargetAfterCurrent(int targetIndex)
        {
            ShiftTurnQueue(CurrentTurnIndex + 1, targetIndex);
        }

        /// <summary>
        /// Shift everything towards the <c>targetIndex</c> in the <c>currentTurnQueue</c>.
        /// This means every element in the list will be moved up or down by 1.
        /// </summary>
        /// <param name="startIndex">Shift everything starting from <c>startIndex</c>.
        /// The Unit in startIndex will not be shifted.</param>
        /// <param name="endIndex">Shift everything until <c>endIndex</c>.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void ShiftTurnQueue(int startIndex, int endIndex)
        {
            if (!UnitCanDoTurnManipulation(ActingUnit))
            {
                Debug.LogWarning($"{ActingUnit} cannot turn manipulate.");
                return;
            }

            if (endIndex < 0 || endIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not move unit at index {endIndex}");

            unitsTurnManipulatedThisRound.Add(currentTurnQueue[CurrentTurnIndex]);
            unitsTurnManipulatedThisRound.Add(currentTurnQueue[endIndex]);
            
            if (startIndex == endIndex)
                return;

            int difference = startIndex - endIndex;
            int increment = difference / Mathf.Abs(difference);
            int currentIndex = endIndex;
            IUnit tempUnit = currentTurnQueue[endIndex];

            while (currentIndex != startIndex)
            {
                currentTurnQueue[currentIndex] = currentTurnQueue[currentIndex + increment];
                currentIndex += increment;
            }

            Insight.Value--;
            currentTurnQueue[startIndex] = tempUnit;
            commandManager.ExecuteCommand(new TurnManipulatedCommand(currentTurnQueue[startIndex],
                currentTurnQueue[endIndex]));
            EndTurnManipulationPhase();
        }

        #endregion

        #region Getters

        /// <summary>
        /// Returns the <c>PlayerUnit</c> whose turn it currently is. Returns null if no
        /// <c>PlayerUnit</c> is acting.
        /// </summary>
        private PlayerUnit GetActingPlayerUnit()
        {
            if (ActingUnit is PlayerUnit currentPlayerUnit)
                return currentPlayerUnit;

            return null;
        }

        /// <summary>
        /// Returns the <c>EnemyUnit</c> whose turn it currently is. Returns null if no
        /// <c>EnemyUnit</c> is acting.
        /// </summary>
        private EnemyUnit GetActingEnemyUnit()
        {
            if (ActingUnit is EnemyUnit currentEnemyUnit)
                return currentEnemyUnit;

            return null;
        }

        #endregion

        #region Boolean Functions

        /// <summary>
        /// Check if there are any enemy units in the queue.
        /// </summary>
        /// <returns>
        /// True if there is at least one <c>EnemyUnit</c> in the <c>currentTurnQueue</c>.
        /// </returns>
        private bool HasEnemyUnitInQueue() => currentTurnQueue.Any(u => u is EnemyUnit);

        /// <summary>
        /// Checks if the acting unit can do the turn phase.
        /// </summary>
        public bool IsTurnManipulationPhase() => PhaseIndex <= TurnManipulationPhaseIndex;

        /// <summary>
        /// Checks if the acting unit can do the movement phase.
        /// </summary>
        public bool IsMovementPhase() => PhaseIndex <= MovementPhaseIndex;

        /// <summary>
        /// Checks if the acting unit can do the ability phase.
        /// </summary>
        public bool IsAbilityPhase() => PhaseIndex <= AbilityPhaseIndex;

        /// <summary>
        /// Checks if the acting unit has completed all turn phases.
        /// </summary>
        private bool LastPhaseHasEnded() => !IsAbilityPhase() &&
                                            !IsMovementPhase() &&
                                            !IsTurnManipulationPhase();

        public bool UnitCanDoTurnManipulation(IUnit unit)
        {
            if (!UnitCanMeditate(unit))
                return false;
            
            if (Insight.Value <= 0)
            {
                Debug.LogWarning($"Not enough insight.");
                return false;
            }

            return true;
        }
        
        public bool UnitCanMeditate(IUnit unit)
        {
            if (!(unit is PlayerUnit))
            {
                Debug.LogWarning($"{unit} is not a {nameof(PlayerUnit)}");
                return false;
            }
            
            if (unitsMeditatedLastRound.Contains(unit))
            {
                Debug.LogWarning($"{unit} meditated last round.");
                return false;
            }
            
            if (unitsMeditatedThisRound.Contains(unit) || unitsTurnManipulatedThisRound.Contains(unit))
            {
                Debug.LogWarning($"{unit} already turn manipulated this round.");
                return false;
            }
            
            if (!IsTurnManipulationPhase())
            {
                Debug.LogWarning($"{unit} is not in the turn manipulation phase.");
                return false;
            }

            return true;
        }

        // TODO: Make sure meditated units cannot be turn manipulated
        public bool UnitCanBeTurnManipulated(IUnit unit) =>
            !unitsMeditatedLastRound.Contains(unit) &&
            !unitsMeditatedThisRound.Contains(unit);


        /// <summary>
        /// Determines whether the given unit is meditating or not
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool UnitIsMeditated(IUnit unit) =>
            !unitsMeditatedLastRound.Contains(unit) &&
            !unitsMeditatedThisRound.Contains(unit);

        /// <summary>
        /// Check if there are any player units in the queue.
        /// </summary>
        /// <returns>
        /// True if there is at least one <c>PlayerUnit</c> in the <c>currentTurnQueue</c>.
        /// </returns>
        private bool HasPlayerUnitInQueue() => currentTurnQueue.Any(u => u is PlayerUnit);

        #endregion

        #region Unit

        /// <summary>
        /// Resets the necessary stats of all units at the end of a round.
        /// </summary>
        private void ResetUnitStatsAfterRound()
        {
            foreach (IUnit unit in unitManager.AllUnits)
            {
                unit.MovementPoints.Reset();
                unit.AttackStat.Reset();
                unit.DefenceStat.Reset();
            }
        }
        
        #endregion
        
        #region Turn Phases
        
        private void SetupTurnPhases(TurnPhases[] newTurnPhases)
        {
            if (newTurnPhases.Length != 3)
            {
                Debug.LogError("Could not set up turn queue. Turn phases list must contain" +
                               " three elements.");
                return;
            }
            
            for (int i = 0; i < 3; i++)
            {
                switch (newTurnPhases[i])
                {
                    case TurnPhases.TurnManipulation:
                        TurnManipulationPhaseIndex = i;
                        break;
                    case TurnPhases.Movement:
                        MovementPhaseIndex = i;
                        break;
                    case TurnPhases.Ability:
                        AbilityPhaseIndex = i;
                        break;
                }
            }

            PhaseIndex = 0;
        }

        private void EndMovementPhase()
        {
            if (IsMovementPhase())
                PhaseIndex = MovementPhaseIndex + 1;
            else
                Debug.LogWarning("Movement was done out of phase.");

            if (LastPhaseHasEnded())
                commandManager.ExecuteCommand(new EndTurnCommand(ActingUnit));
        }

        private void EndAbilityPhase()
        {
            if (IsAbilityPhase())
                PhaseIndex = AbilityPhaseIndex + 1;
            else
                Debug.LogWarning("Ability was done out of phase.");

            if (LastPhaseHasEnded())
                commandManager.ExecuteCommand(new EndTurnCommand(ActingUnit));
        }

        private void EndTurnManipulationPhase()
        {
            if (IsTurnManipulationPhase())
                PhaseIndex = TurnManipulationPhaseIndex + 1;
            else
                Debug.LogWarning("Turn manipulation was done out of phase.");

            if (LastPhaseHasEnded())
                commandManager.ExecuteCommand(new EndTurnCommand(ActingUnit));
        }

        #endregion
    }
}
