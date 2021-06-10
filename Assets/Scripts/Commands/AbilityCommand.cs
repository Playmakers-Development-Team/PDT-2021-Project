using Abilities;
using Units;
using UnityEngine;

namespace Commands
{
    public class AbilityCommand : HistoricalCommand
    {
        private readonly Ability ability;
        public Vector2 TargetVector { get; }
        public Vector2Int OriginCoordinate { get; }
        public IUnit Unit { get; }
        
        public AbilityCommand(IUnit unit, Vector2 targetVector, Ability ability)
        {
            this.ability = ability;
            this.TargetVector = targetVector;
            this.OriginCoordinate = unit.Coordinate;
            Unit = unit;
        }

        public override void Execute()
        {
            ability.Use(Unit, OriginCoordinate, TargetVector);
        }

        public override void Undo()
        {
            ability.Undo(Unit, OriginCoordinate, TargetVector);
        }
    }
}
