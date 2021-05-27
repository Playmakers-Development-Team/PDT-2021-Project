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
        /// An event that triggers when a round has ended
        /// </summary>
        public event Action<TurnManager> onTurnEnd;

        /// <summary>
        /// An event that triggers when a round has started
        /// </summary>
        public event Action<TurnManager> onRoundStart; 
        
        
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
        public int TurnIndex { get; private set; }
        
        /// <summary>
        /// The unit that is currently taking its turn.
        /// </summary>
        public IUnit CurrentUnit => currentTurnQueue[TurnIndex];

        public IUnit PreviousUnit => GetPreviousUnit();

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

        
        
        //private PlayerManager playerManager;
        private CommandManager commandManager;
        private UnitManager unitManager;
        
        private List<IUnit> previousTurnQueue = new List<IUnit>();
        private List<IUnit> currentTurnQueue = new List<IUnit>();
        private List<IUnit> nextTurnQueue = new List<IUnit>();
        
        
        
        
        
        public override void ManagerStart()
        {
            //playerManager = ManagerLocator.Get<PlayerManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
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
            TurnIndex = 0;
            previousTurnQueue = new List<IUnit>();
            UpdateNextTurnQueue();
            currentTurnQueue = new List<IUnit>(nextTurnQueue);
            
            // TODO might want to register listeners e.g EndTurnCommand here
        }

        // TODO Test
        /// <summary>
        /// Remove a unit completely from the current turn queue and future turn queues.
        /// For situations such as when a unit is killed.
        /// </summary>
        /// <param name="targetIndex">The index of the unit.</param>
        /// <exception cref="IndexOutOfRangeException">If the index is not valid.</exception>
        public void RemoveUnitFromQueue(int targetIndex)
        {
            if (targetIndex < 0 || targetIndex >= CurrentTurnQueue.Count)
                throw new IndexOutOfRangeException($"Could not remove unit at index {targetIndex}");

            if (TurnIndex >= targetIndex)
                TurnIndex--;

            bool removingCurrentUnit = targetIndex == TurnIndex;
            currentTurnQueue.RemoveAt(targetIndex);
            UpdateNextTurnQueue();
            
            if (removingCurrentUnit)
                NextTurn();
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
            if (TurnIndex < 2 || TurnIndex == targetIndex || TurnIndex == targetIndex - 1)
                return;

            int aboveIndex = TurnIndex - 1;

            ShiftTurnQueue(aboveIndex, targetIndex);
            
            // Set the current turn to be the unit before first, later coming back to the current unit
            TurnIndex = aboveIndex;
            commandManager.QueueCommand(new StartTurnCommand(CurrentUnit));
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
            if (TurnIndex >= currentTurnQueue.Count - 1 || TurnIndex == targetIndex || TurnIndex == targetIndex + 1)
                return;

            int belowIndex = TurnIndex + 1;
            ShiftTurnQueue(belowIndex, targetIndex);
        }
        
        /// <summary>
        /// Create a turn queue from every available <c>Unit</c> in <c>PlayerManager</c> and
        /// <c>EnemyManager</c>. Calculate the turn order based on the parameters.
        /// </summary>
        private List<IUnit> CreateTurnQueue()
        {
            // TODO Do the same thing for enemies
            List<IUnit> turnQueue = new List<IUnit>();
            turnQueue.AddRange(unitManager.GetAllUnits());
            for (int i = 0; i < turnQueue.Count - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (turnQueue[j - 1].Speed > turnQueue[j].Speed)
                    {
                        IUnit temp = turnQueue[j - 1];
                        turnQueue[j - 1] = turnQueue[j];
                        turnQueue[j] = temp;
                    }
                }
            }

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
        /// Gets the previous unit
        /// </summary>
        /// <returns> returns the previous unit unless the unit was first</returns>
        private IUnit GetPreviousUnit() 
        {
            return TurnIndex == 0 ? null : currentTurnQueue[TurnIndex - 1];
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
            TurnIndex++;
            TotalTurnCount++;
            if (TurnIndex < currentTurnQueue.Count)
            {
                Debug.Log(CurrentUnit.ToString());
                commandManager.QueueCommand(new StartTurnCommand(CurrentUnit));
            }
            else
            {
                NextRound();
            }
            
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
            TurnIndex = 0;
            
            commandManager.QueueCommand(new StartTurnCommand(CurrentUnit));

            // TODO Add option for a draw
            if (!HasEnemyUnitInQueue())
            {
                // TODO Player wins. End the encounter somehow, probably inform the GameManager
            }

            if (!HasPlayerUnitInQueue())
            {
                // TODO Player loses. End the encounter somehow, probably inform the GameManager
            }

            previousTurnQueue = currentTurnQueue;
            currentTurnQueue = nextTurnQueue;
            nextTurnQueue = CreateTurnQueue();
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
