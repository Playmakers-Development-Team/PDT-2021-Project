using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Units;
using UnityEngine;

namespace Managers
{
    public class TurnManager : Manager
    {
        /// <summary>
        /// An event that triggers when a round has ended.
        /// </summary>
        public event Action<TurnManager> onTurnEnd;

        /// <summary>
        /// An event that triggers when a round has started.
        /// </summary>
        public event Action<TurnManager> onRoundStart;

        /// <summary>
        /// An event that triggers when a unit has died.
        /// </summary>
        public event Action<TurnManager> onUnitDeath;

        /// <summary>
        /// An event that triggers when a new unit has spawned.
        /// </summary>
        public event Action<TurnManager> newUnitAdded;
        
        /// <summary>
        /// Gives how many turns have passed throughout the entire level.
        /// </summary>
        public int TotalTurnCount { get; private set; }

        /// <summary>
        /// Gives how many rounds have passed.
        /// </summary>
        public int RoundCount { get; private set; }

        /// <summary>
        /// The index of the unit that is currently taking its turn.
        /// </summary>
        public int CurrentTurnIndex { get; private set; }

        [Obsolete("Use CurrentTurnIndex")]
        public int TurnIndex => CurrentTurnIndex;

        /// <summary>
        /// The unit that is currently taking its turn.
        /// </summary>
        public IUnit CurrentUnit => currentTurnQueue[CurrentTurnIndex];

        /// <summary>
        /// The unit that took its turn before the current unit.
        /// </summary>
        public IUnit PreviousUnit => CurrentTurnIndex == 0 ? null : currentTurnQueue[CurrentTurnIndex - 1];

        /// <summary>
        /// The unit that most recently died.
        /// </summary>
        public IUnit RecentUnitDeath { get; private set; }

        /// <summary>
        /// The order in which units will take their turns for the current round.
        /// </summary>
        public IReadOnlyList<IUnit> CurrentTurnQueue => currentTurnQueue.AsReadOnly();

        /// <summary>
        /// The order in which units will take their turns for the next round.
        /// </summary>
        public IReadOnlyList<IUnit> NextTurnQueue => nextTurnQueue.AsReadOnly();

        /// <summary>
        /// The order in which units took their turns for the previous round.
        /// </summary>
        public IReadOnlyList<IUnit> PreviousTurnQueue => previousTurnQueue.AsReadOnly();
        
        private CommandManager commandManager;
        private PlayerManager playerManager;
        private UnitManager unitManager;
        
        private List<IUnit> previousTurnQueue = new List<IUnit>();
        private List<IUnit> currentTurnQueue = new List<IUnit>();
        private List<IUnit> nextTurnQueue = new List<IUnit>();

        /// <summary>
        /// Checks if the next turn timeline needs to be updated due to a change in the current
        /// timeline.
        /// </summary>
        private bool timelineNeedsUpdating = false;

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            unitManager = ManagerLocator.Get<UnitManager>();

            commandManager.ListenExecuteCommand<EndTurnCommand>((cmd) => NextTurn());
        }

        // TODO Call this function when level is loaded
        /// <summary>
        /// Create a turn queue based on existing player and enemy units.
        /// Should be called after the level is loaded and all the units are ready.
        /// </summary>
        public void SetupTurnQueue()
        {
            RoundCount = 0;
            TotalTurnCount = 0;
            CurrentTurnIndex = 0;
            previousTurnQueue = new List<IUnit>();
            UpdateNextTurnQueue();
            currentTurnQueue = new List<IUnit>(nextTurnQueue);
        }

        /// <summary>
        /// Find the turn index of a unit of the current turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromCurrentQueue(IUnit unit)
        {
            return currentTurnQueue.FindIndex(u => u == unit);
        }
        
        /// <summary>
        /// Find the turn index of a unit of the previous turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromPreviousQueue(IUnit unit)
        {
            return previousTurnQueue.FindIndex(u => u == unit);
        }
        
        /// <summary>
        /// Find the turn index of a unit of the next turn queue.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <returns>The turn index of the unit or -1 if not found.</returns>
        public int FindTurnIndexFromNextQueue(IUnit unit)
        {
            return nextTurnQueue.FindIndex(u => u == unit);
        }

        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="unit">Target unit</param>
        /// <exception cref="IndexOutOfRangeException">If the unit is not in the turn queue.</exception>
        public void RemoveUnitFromQueue(IUnit unit)
        {
            RemoveUnitFromQueue(FindTurnIndexFromCurrentQueue(unit));
        }
        
        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="targetIndex">The index of the unit.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void RemoveUnitFromQueue(int targetIndex)
        {
            // Debug.Log("Target Index: " + targetIndex);
            
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not remove unit at index {targetIndex}");
            
            //bool removingCurrentUnit = targetIndex == CurrentTurnIndex; redundant
            
            // If we're removing something, the list becomes smaller and therefore we need to 
            // decrement the CurrentTurnIndex to point to the same unit.
            // If the unit removed is the current unit, then we want to decrement it so we can
            // call NextTurn() later. [I have made this redundant as the index not moving inheritenly changes the next turn (However there should be checks for endgameconditions)]
            //or if units interact with next turns
            //Set an additional condition to make sure that there is a previous unit
            if (targetIndex <= CurrentTurnIndex && PreviousUnit != null)
            {
                if (PreviousUnit != currentTurnQueue[CurrentTurnIndex - 1] )
                    CurrentTurnIndex--;
                
                else if (targetIndex <= CurrentTurnIndex && PreviousUnit == currentTurnQueue[targetIndex])
                    CurrentTurnIndex--;
            }

            RecentUnitDeath = currentTurnQueue[targetIndex];
            currentTurnQueue.RemoveAt(targetIndex);
            UpdateNextTurnQueue();
            timelineNeedsUpdating = true;
                
            // reselects the unit if the current unit has died
            if (CurrentUnit is PlayerUnit)
            {
                playerManager.SelectUnit((PlayerUnit) CurrentUnit);
            }
            else
            {
                playerManager.DeselectUnit();
            }

            onUnitDeath?.Invoke(this);
        }

        // TODO Test
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
            commandManager.ExecuteCommand(new StartTurnCommand(CurrentUnit));
        }

        // TODO Test
        /// <summary>
        /// Move a unit right after the current unit. The moved unit will take a turn after the
        /// current unit.
        /// </summary>
        /// <param name="targetIndex">The index of the unit to be moved.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void MoveTargetAfterCurrent(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not move unit at index {targetIndex}");
            
            // BUG Cannot move target to last position
            if (CurrentTurnIndex >= currentTurnQueue.Count - 1 || CurrentTurnIndex == targetIndex || CurrentTurnIndex == targetIndex + 1)
                return;

            int belowIndex = CurrentTurnIndex + 1;
            ShiftTurnQueue(belowIndex, targetIndex);
        }
        
        /// <summary>
        /// Create a turn queue from every available <c>Unit</c> in <c>PlayerManager</c> and
        /// <c>EnemyManager</c>. Calculate the turn order based on the parameters.
        /// </summary>
        private List<IUnit> CreateTurnQueue()
        {
            List<IUnit> turnQueue = new List<IUnit>();
            
            turnQueue.AddRange(unitManager.GetAllUnits());
            
            turnQueue.Sort((x, y) => x.Speed.Value.CompareTo(y.Speed.Value));

            return turnQueue;
        }

        /// <summary>
        /// Should be called whenever the number of units in the turn queue has been changed.
        /// </summary>
        private void UpdateNextTurnQueue()
        {
            nextTurnQueue = CreateTurnQueue();
            // TODO might want to update UI here
        }

        /// <summary>
        /// Adds a new unit to the timeline and setting it to the end of the current turn queue
        /// </summary>
        public void AddNewUnitToTimeline(IUnit unit)
        {
            currentTurnQueue.Add(unit);
            nextTurnQueue.Add(unit);  // No purpose, since nextTurnQueue will be recalculated
            newUnitAdded?.Invoke(this);
            timelineNeedsUpdating = true;
        }

        /// <summary>
        /// Shift everything towards the <c>targetIndex</c> in the <c>currentTurnQueue</c>.
        /// This means every element in the list will be moved up or down by 1.
        /// </summary>
        /// <param name="startIndex">Shift everything starting from <c>startIndex</c>.
        /// The Unit in startIndex will not be shifted.</param>
        /// <param name="endIndex">Shift everything until <c>endIndex</c>.</param>
        private void ShiftTurnQueue(int startIndex, int endIndex)
        {
            if (startIndex == endIndex)
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

        /// <summary>
        /// Finish the current turn and end the round if this is the last turn.
        /// </summary>
        public void NextTurn()
        {
            CurrentTurnIndex++;
            TotalTurnCount++;
            
            if (CurrentTurnIndex >= currentTurnQueue.Count)
            {
                NextRound();
                
            }
            
            // Debug.Log(CurrentUnit.ToString());
            commandManager.ExecuteCommand(new StartTurnCommand(CurrentUnit));
            
            if (CurrentUnit is PlayerUnit)
            {
                playerManager.SelectUnit((PlayerUnit) CurrentUnit);
            }
            else
            {
                playerManager.DeselectUnit();
            }
            
            Debug.Log("next turn has started");
            
            onTurnEnd?.Invoke(this);
        }
        
        /// <summary>
        /// Finish the current round. May transition to the next round or finish the encounter if
        /// there are no enemy or player units remaining. 
        /// </summary>
        private void NextRound()
        {
            // TODO might want to call the next round command or something here
            RoundCount++;
           
            
            // TODO Add option for a draw
            if (!HasEnemyUnitInQueue())
            {
                Debug.Log("YOU WIN!"); // added these debugs for testing timeline
                // TODO Player wins. End the encounter somehow, probably inform the GameManager
            }

            if (!HasPlayerUnitInQueue())
            {
                Debug.Log("YOU LOSE!");

                // TODO Player loses. End the encounter somehow, probably inform the GameManager
            }

            previousTurnQueue = currentTurnQueue;

            // if a new unit was spawned, then new turn queue needs to be updated to accompany the new unit
            currentTurnQueue = timelineNeedsUpdating ? CreateTurnQueue() : nextTurnQueue;
            timelineNeedsUpdating = false;
            nextTurnQueue = CreateTurnQueue();
            CurrentTurnIndex = 0;
            onRoundStart?.Invoke(this);
        }

        /// <summary>
        /// Check if there are any enemy units in the queue.
        /// </summary>
        /// <returns>
        /// True if there is at least one <c>EnemyUnit</c> in the <c>currentTurnQueue</c>.
        /// </returns>
        private bool HasEnemyUnitInQueue()
        {
            return currentTurnQueue.Any(u => u is EnemyUnit);
        }
        
        /// <summary>
        /// Check if there are any player units in the queue.
        /// </summary>
        /// <returns>
        /// True if there is at least one <c>PlayerUnit</c> in the <c>currentTurnQueue</c>.
        /// </returns>
        private bool HasPlayerUnitInQueue()
        {
            return currentTurnQueue.Any(u => u is PlayerUnit);
        }
    }
}
