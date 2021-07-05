using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Units;
using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Manager
    {
        #region Properties and Fields
        
        public enum TurnPhases
        {
            TurnManipulation, 
            Movement, 
            Ability
        };
        public int TotalTurnCount { get; private set; }
        public int RoundCount { get; private set; }
        public int CurrentTurnIndex { get; private set; }
        public int TurnManipulationPhaseIndex { get; private set; }
        public int MovementPhaseIndex { get; private set; }
        public int AbilityPhaseIndex { get; private set; }
        public int PhaseIndex { get; set; }
        public IUnit ActingUnit => currentTurnQueue[CurrentTurnIndex]; // The unit that is currently taking its turn
        public IUnit PreviousActingUnit => CurrentTurnIndex == 0 ? null : currentTurnQueue[CurrentTurnIndex - 1];
        public IUnit RecentUnitDeath { get; private set; }
        public IReadOnlyList<IUnit> CurrentTurnQueue => currentTurnQueue.AsReadOnly();
        public IReadOnlyList<IUnit> NextTurnQueue => nextTurnQueue.AsReadOnly();
        public IReadOnlyList<IUnit> PreviousTurnQueue => previousTurnQueue.AsReadOnly();
        public PlayerUnit ActingPlayerUnit => GetActingPlayerUnit();
        public EnemyUnit ActingEnemyUnit => GetActingEnemyUnit();
        
        private CommandManager commandManager;
        private PlayerManager playerManager;
        private UnitManager unitManager;
        private EnemyManager enemyManager;
        
        private List<IUnit> previousTurnQueue = new List<IUnit>();
        private List<IUnit> currentTurnQueue = new List<IUnit>();
        private List<IUnit> nextTurnQueue = new List<IUnit>();
        private bool timelineNeedsUpdating;

        #endregion

        #region Manager Overrides
        
        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();

            commandManager.ListenCommand<EndTurnCommand>(cmd => NextTurn());
            commandManager.ListenCommand<SpawnedUnitCommand>(cmd => AddNewUnitToTimeline(cmd.Unit));
            commandManager.ListenCommand<KilledUnitCommand>(cmd => RemoveUnitFromQueue(cmd.Unit));
          
        }
        
        #endregion

        #region Turn Queue Manipulation
        
        /// <summary>
        /// Create a turn queue based on existing player and enemy units.
        /// Should be called after the level is loaded and all the units are ready.
        /// </summary>
        public void SetupTurnQueue(TurnPhases[] newTurnPhases)
        {

            for (int i = 0; i < 3; i++)
            {
                switch (newTurnPhases[i].ToString())
                {
                    case "TurnManipulation":
                        TurnManipulationPhaseIndex = i;
                        break;
                    case "Movement":
                        MovementPhaseIndex = i;
                        break;
                    case "Ability":
                        AbilityPhaseIndex = i;
                        break;
                }
            }

            PhaseIndex = 0;
            RoundCount = 0;
            TotalTurnCount = 0;
            CurrentTurnIndex = 0;
            previousTurnQueue = new List<IUnit>();
            UpdateNextTurnQueue();
            currentTurnQueue = new List<IUnit>(nextTurnQueue);
            
            if (!(ActingEnemyUnit is null))
                enemyManager.DecideEnemyIntention(ActingEnemyUnit);
            
            commandManager.ExecuteCommand(new TurnQueueCreatedCommand());
        }
        
        
    
        
        /// <summary>
        /// Create a turn queue from every available <c>Unit</c> in <c>PlayerManager</c> and
        /// <c>EnemyManager</c>. Calculate the turn order based on speed.
        /// </summary>
        private List<IUnit> CreateTurnQueue()
        {
            List<IUnit> turnQueue = new List<IUnit>();
            turnQueue.AddRange(unitManager.AllUnits);
            turnQueue.Sort((x, y) => x.Speed.Value.CompareTo(y.Speed.Value));
            return turnQueue;
        }
        
        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="targetUnit">The unit to remove.</param>
        /// <exception cref="ArgumentException">If the unit does not exist in the queue.</exception>
        private void RemoveUnitFromQueue(IUnit targetUnit)
        {
            var targetIndex = FindTurnIndexFromCurrentQueue(targetUnit);

            if (targetIndex == -1)
                throw new ArgumentException($"The unit {targetUnit} does not exist in the queue and could not be removed.");
            
            RemoveUnitFromQueue(targetIndex);
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
            
            if (targetIndex <= CurrentTurnIndex && PreviousActingUnit != null)
                CurrentTurnIndex--;
            
            RecentUnitDeath = currentTurnQueue[targetIndex];
            currentTurnQueue.RemoveAt(targetIndex);
            UpdateNextTurnQueue();
            timelineNeedsUpdating = true;
            SelectCurrentUnit(); // Reselect the new current unit if the old current unit has died
        }
        
        /// <summary>
        /// Should be called whenever the number of units in the turn queue has been changed.
        /// </summary>
        private void UpdateNextTurnQueue() => nextTurnQueue = CreateTurnQueue();
        
        /// <summary>
        /// Adds a new unit to the timeline and setting it to the end of the current turn queue.
        /// </summary>
        private void AddNewUnitToTimeline(IUnit unit)
        {
            currentTurnQueue.Add(unit);
            timelineNeedsUpdating = true;
        }
        
        /// <summary>
        /// Finish the current turn and end the round if this is the last turn.
        /// </summary>
        public void NextTurn()
        {
            CurrentTurnIndex++;
            TotalTurnCount++;
            
            if (CurrentTurnIndex >= currentTurnQueue.Count)
                NextRound();
            
            StartTurn();
        }

        private void StartTurn()
        {
            commandManager.ExecuteCommand(new StartTurnCommand(ActingUnit));

            if (!(ActingEnemyUnit is null))
                enemyManager.DecideEnemyIntention(ActingEnemyUnit);
            else
                PhaseIndex = 0;
            
            SelectCurrentUnit();
        }
        
        /// <summary>
        /// Finish the current round. May transition to the next round or finish the encounter if
        /// there are no enemy or player units remaining. 
        /// </summary>
        private void NextRound()
        {
            RoundCount++;
            commandManager.ExecuteCommand(new PrepareRoundCommand());
            
            // TODO Add option for a draw
            if (!HasEnemyUnitInQueue())
            {
                // Debug.Log("YOU WIN!");
                // TODO Player wins. End the encounter somehow, probably inform the GameManager
                // Sets the audio to out of combat version. TODO Move this to the GameManager or MusicManager
                AkSoundEngine.SetState("CombatState", "Out_Of_Combat");
            }

            if (!HasPlayerUnitInQueue())
            {
               // Debug.Log("YOU LOSE!");
               // TODO Player wins. End the encounter somehow, probably inform the GameManager
               // Sets the audio to out of combat version. TODO Move this to the GameManager or MusicManager
               AkSoundEngine.SetState("CombatState", "Out_Of_Combat");
            }

            previousTurnQueue = currentTurnQueue;
            currentTurnQueue = timelineNeedsUpdating ? CreateTurnQueue() : nextTurnQueue;   // if a new unit was spawned, then new turn queue needs to be updated to accompany the new unit
            timelineNeedsUpdating = false;
            nextTurnQueue = CreateTurnQueue();
            CurrentTurnIndex = 0;
            ResetUnitStatsAfterRound();
            commandManager.ExecuteCommand(new StartRoundCommand());
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
        
        /// <summary>
        /// Move a unit right before the current unit. The moved unit will take a turn instantly
        /// before continuing to the current unit.
        /// </summary>
        /// <param name="targetIndex">The index of the unit to be moved.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void MoveTargetBeforeCurrent(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not move unit at index {targetIndex}");
            
            // BUG Cannot move target to first position
            if (CurrentTurnIndex < 2 || CurrentTurnIndex == targetIndex || CurrentTurnIndex == targetIndex - 1)
                return;

            int aboveIndex = CurrentTurnIndex - 1;

            ShiftTurnQueue(aboveIndex, targetIndex);
            
            // Set the current turn to be the unit before first, later coming back to the current unit
            CurrentTurnIndex = aboveIndex;

            // TODO: Test
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
            //TODO: Test
            
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not move unit at index {targetIndex}");
            
            // BUG Cannot move target to last position
            if (CurrentTurnIndex >= currentTurnQueue.Count - 1 || CurrentTurnIndex == targetIndex || CurrentTurnIndex == targetIndex + 1)
                return;

            int belowIndex = CurrentTurnIndex + 1;
            ShiftTurnQueue(belowIndex, targetIndex);
        }
        
        /// <summary>
        /// Shift everything towards the <c>targetIndex</c> in the <c>currentTurnQueue</c>.
        /// This means every element in the list will be moved up or down by 1.
        /// </summary>
        /// <param name="startIndex">Shift everything starting from <c>startIndex</c>.
        /// The Unit in startIndex will not be shifted.</param>
        /// <param name="endIndex">Shift everything until <c>endIndex</c>.</param>
        public void ShiftTurnQueue(int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
                return;
            
            if (TurnManipulationPhaseIndex < PhaseIndex )
            {
                //TODO DELETE DEBUG
                Debug.Log("Unable to do Turn Manipulation phase, it has been completed");
                return;
            }

            if (TurnManipulationPhaseIndex > PhaseIndex)
                PhaseIndex = TurnManipulationPhaseIndex + 1;
            else
                PhaseIndex++;
            
            //TODO DELETE DEBUG AND RETURN STATEMENT (RETURN STATEMENT IS SO THE FOLLOWING CODE DOES NOT RUN)
            Debug.Log("Turn Manipulation started");
            return;
            
            int difference = endIndex - startIndex;
            int increment = difference / Mathf.Abs(difference);
            int currentIndex = startIndex + increment;
            
            while (currentIndex != endIndex)
            {
                currentIndex += increment;
                currentTurnQueue[currentIndex] = currentTurnQueue[currentIndex - increment];
            }
            
            currentTurnQueue[startIndex] = currentTurnQueue[endIndex];
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
        /// Checks if the current player unit can do the turn phase
        /// </summary>
        /// <returns></returns>
        public bool IsTurnPhase() => TurnManipulationPhaseIndex >= PhaseIndex;
        
        /// <summary>
        /// Checks if the current player unit can do the movement phase
        /// </summary>
        /// <returns></returns>
        public bool IsMovementPhase()  => MovementPhaseIndex >= PhaseIndex;
        
        /// <summary>
        /// Checks if the current player unit can do the ability phase
        /// </summary>
        /// <returns></returns>
        public bool IsAbilityPhase()  => AbilityPhaseIndex >= PhaseIndex;
        
        
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
        /// Selects the <c>CurrentUnit</c> if it is of type <c>PlayerUnit</c>.
        /// </summary>
        private void SelectCurrentUnit()
        {
            if (ActingUnit is PlayerUnit)
                playerManager.SelectUnit((PlayerUnit) ActingUnit);
            else
                playerManager.DeselectUnit();
        }
        
        /// <summary>
        /// Resets the necessary stats of all units at the end of a round.
        /// </summary>
        private void ResetUnitStatsAfterRound()
        {
            foreach (IUnit unit in unitManager.AllUnits)
            {
                unit.MovementActionPoints.Reset();
                unit.Attack.Reset();
                unit.Health.Defence.Reset();
            }
        }
        
        #endregion
    }
}
