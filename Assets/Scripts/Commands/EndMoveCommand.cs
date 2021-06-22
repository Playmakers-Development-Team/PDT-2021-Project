using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    /// <summary>
    /// Executed when the unit has finished moving.
    /// </summary>
    public class EndMoveCommand : HistoricalCommand
    {
        public StartMoveCommand startMoveCommand { get; }

        public Vector2Int TargetCoords => startMoveCommand.TargetCoords;

        public Vector2Int CurrentCoords => startMoveCommand.StartCoords;
        
        public EndMoveCommand(StartMoveCommand startMoveCommand) : base(startMoveCommand.Unit)
        {
            this.startMoveCommand = startMoveCommand;
        }

        public override void Execute() {}

        public override void Undo()
        {
            startMoveCommand.Undo();
        }
    }
}
