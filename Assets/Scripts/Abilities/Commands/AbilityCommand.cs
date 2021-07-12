using Commands;
using Units;
using UnityEngine;

namespace Abilities.Commands
{
    public class AbilityCommand : HistoricalCommand
    {
        private readonly Ability ability;
        public Vector2 TargetVector { get; }
        public Vector2Int OriginCoordinate { get; }
        
        public AbilityCommand(IUnit unit, Vector2 targetVector, Ability ability) : base(unit)
        {
            this.ability = ability;
            TargetVector = targetVector;
            OriginCoordinate = unit.Coordinate;
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
