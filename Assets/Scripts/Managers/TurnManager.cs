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
        private readonly List<IUnit> turnQueue = new List<IUnit>();
        
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
        public IUnit CurrentUnit => turnQueue[TurnIndex];

        /// <summary>
        /// The order in which turns between Units execute
        /// </summary>
        public IReadOnlyList<IUnit> TurnQueue => turnQueue.AsReadOnly();

        /// <summary>
        /// Take every available Unit in the scene, calculate the turn orders based on the parameters
        /// and finally create a turn queue.
        /// </summary>
        private void CalculateQueueOrder()
        {
            // UnitManager unitManager = ManagerLocator.Get<UnitManager>();
            // turnQueue.AddRange(unitManager.Units);
            // TODO get all units from unit manager and make a turn order queue
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
            IUnit targetUnit = turnQueue[targetIndex];
            ShiftTurnQueue(aboveIndex, targetIndex);
            turnQueue[aboveIndex] = targetUnit;
            
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
            if (TurnIndex >= turnQueue.Count - 1 || TurnIndex == targetIndex || TurnIndex == targetIndex + 1)
                return;

            int belowIndex = TurnIndex + 1;
            IUnit targetUnit = turnQueue[targetIndex];
            ShiftTurnQueue(belowIndex, targetIndex);
            turnQueue[belowIndex] = targetUnit;
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
                turnQueue[currentIndex] = turnQueue[currentIndex - increment];
            }
        }

        /// <summary>
        /// Finish the current turn, and end the round if this is the last Unit.
        /// </summary>
        public void EndTurn()
        {
            TurnCount++;

            if (TurnCount < turnQueue.Count)
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
        /// Check if there are any enemy Units.
        /// </summary>
        /// <returns>True if there are no <c>EnemyUnit</c> in the turnQueue</returns>
        public bool HasEnemyUnitInQueue()
        {
            return turnQueue.Any(u => u is EnemyUnit);
        }

        /// <summary>
        /// Finish the current Round. Either transition to the next round or if there are no enemies
        /// left, finish the encounter. 
        /// </summary>
        private void EndRound()
        {
            // TODO might want to call the next round command or something
            RoundCount++;
            TurnCount = 0;

            if (!HasEnemyUnitInQueue())
            {
                // TODO End the encounter somehow, probably inform the GameManager
            }
            
            CalculateQueueOrder();
        }
    }
}
