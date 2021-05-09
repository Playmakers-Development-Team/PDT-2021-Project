using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Units;
using UnityEngine;

namespace Managers
{
    public class TurnManager : IManager
    {
        private List<IUnit> previousTurnQueue = new List<IUnit>();
        private List<IUnit> currentTurnQueue = new List<IUnit>();
        private List<IUnit> nextTurnQueue = new List<IUnit>();
        
        /// <summary>
        /// Gives how many round has passed
        /// </summary>
        public int RoundCount { get; private set; }
        
        /// <summary>
        /// Gives how many turns has passed
        /// </summary>
        public int TurnCount { get; private set; }

        /// <summary>
        /// The index of the Unit that is currently having a turn
        /// </summary>
        public int TurnIndex { get; private set; }
        
        /// <summary>
        /// The Unit that is currently having a turn
        /// </summary>
        public IUnit CurrentUnit => currentTurnQueue[TurnIndex];

        /// <summary>
        /// The order in which turns between Units execute
        /// </summary>
        public IReadOnlyList<IUnit> CurrentTurnQueue => currentTurnQueue.AsReadOnly();

        /// <summary>
        /// The order in which turns between Units execute for the next round
        /// </summary>
        public IReadOnlyList<IUnit> NextTurnQueue => nextTurnQueue.AsReadOnly();

        /// <summary>
        /// The order in which turns between Units execute for the previous round
        /// </summary>
        public IReadOnlyList<IUnit> PreviousTurnQueue => previousTurnQueue.AsReadOnly();

        // TODO Call this function when level is loaded
        /// <summary>
        /// Create a turn queue based on existing player and enemy units.
        /// Should be called when the level is loaded after all the units are ready.
        /// </summary>
        public void SetupTurnQueue()
        {
            previousTurnQueue = new List<IUnit>();
            UpdateNextTurnQueue();
            currentTurnQueue = new List<IUnit>(nextTurnQueue);
            
            // TODO might want to register listeners e.g EndTurnCommand here
        }
        
        /// <summary>
        /// Create a turn queue from every available Unit in the scene, calculate the turn orders
        /// based on the parameters.
        /// </summary>
        private List<IUnit> CreateTurnQueue()
        {
            // TODO Do the same thing for enemies
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            
            List<IUnit> turnQueue = new List<IUnit>();
            turnQueue.AddRange(playerManager.PlayerUnits);
            
            // TODO sort the list based on some parameters

            return turnQueue;
        }

        /// <summary>
        /// Should be called whenever the units in the turn queue has been changed.
        /// </summary>
        private void UpdateNextTurnQueue()
        {
            nextTurnQueue = CreateTurnQueue();
        }

        /// <summary>
        /// Move a Unit right before the current Unit. That Unit would take a turn instantly before
        /// continuing to the current Unit.
        /// </summary>
        /// <param name="targetIndex">The index of the Unit</param>
        public void MoveTargetBeforeCurrent(int targetIndex)
        {
            if (TurnIndex < 2 || TurnIndex == targetIndex || TurnIndex == targetIndex - 1)
                return;

            int aboveIndex = TurnIndex - 1;
            IUnit targetUnit = currentTurnQueue[targetIndex];
            ShiftTurnQueue(aboveIndex, targetIndex);
            currentTurnQueue[aboveIndex] = targetUnit;
            
            // Set the current turn to be the Unit before first, later coming back to the current unit
            TurnIndex = aboveIndex;
            CommandManager commandManager = ManagerLocator.Get<CommandManager>();
            commandManager.QueueCommand(new StartTurnCommand(CurrentUnit));
        }

        /// <summary>
        /// Move a Unit right after the current Unit. That Unit would take a turn after the current
        /// Unit is done with its turn.
        /// </summary>
        /// <param name="targetIndex">The index of the Unit</param>
        public void MoveTargetAfterCurrent(int targetIndex)
        {
            if (TurnIndex >= currentTurnQueue.Count - 1 || TurnIndex == targetIndex || TurnIndex == targetIndex + 1)
                return;

            int belowIndex = TurnIndex + 1;
            IUnit targetUnit = currentTurnQueue[targetIndex];
            ShiftTurnQueue(belowIndex, targetIndex);
            currentTurnQueue[belowIndex] = targetUnit;
        }

        /// <summary>
        /// Shift everything towards the <c>targetIndex</c> in the <c>turnQueue</c>.
        /// This means every element in the list will be moved up or down by 1.
        /// </summary>
        /// <param name="startIndex">Shift everything starting from <c>startIndex</c>.
        /// The Unit in startIndex will not be shifted</param>
        /// <param name="targetIndex">Shift everything until <c>targetIndex</c></param>
        private void ShiftTurnQueue(int startIndex, int targetIndex)
        {
            if (startIndex == targetIndex)
                return;

            int difference = targetIndex - startIndex;
            int increment = difference / Mathf.Abs(difference);
            int currentIndex = startIndex + increment;

            while (currentIndex != targetIndex)
            {
                currentIndex += increment;
                currentTurnQueue[currentIndex] = currentTurnQueue[currentIndex - increment];
            }
            
            UpdateNextTurnQueue();
        }

        // TODO listen to EndTurnCommand somehow
        /// <summary>
        /// Finish the current turn, and end the round if this is the last Unit.
        /// </summary>
        private void EndTurn()
        {
            TurnCount++;

            if (TurnCount < currentTurnQueue.Count)
            {
                CommandManager commandManager = ManagerLocator.Get<CommandManager>();
                commandManager.QueueCommand(new StartTurnCommand(CurrentUnit));
            }
            else
            {
                EndRound();
            }
        }
        
        /// <summary>
        /// Finish the current Round. May transition to the next round. If there are no enemies units
        /// or no player units left, finish the encounter. 
        /// </summary>
        private void EndRound()
        {
            // TODO might want to call the next round command or something here
            RoundCount++;
            TurnCount = 0;

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
        }

        /// <summary>
        /// Check if there are any enemy Units.
        /// </summary>
        /// <returns>True if there are no <c>EnemyUnit</c> in the turnQueue</returns>
        public bool HasEnemyUnitInQueue()
        {
            return currentTurnQueue.Any(u => u is EnemyUnit);
        }
        
        /// <summary>
        /// Check if there are any player units.
        /// </summary>
        /// <returns>True if there are no <c>PlayerUnit</c> in the turnQueue</returns>
        public bool HasPlayerUnitInQueue()
        {
            return currentTurnQueue.Any(u => u is PlayerUnit);
        }
    }
}
