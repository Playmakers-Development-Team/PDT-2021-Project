using Abilities;
using Units;
using UnityEngine;

namespace Commands
{
    public class AbilityCommand : Command
    {
        private readonly Ability ability;
        private readonly Vector2 targetVector;
        private readonly Vector2Int originCoordinate;
        private readonly IUnit unit;

        public AbilityCommand(IUnit unit, Vector2 targetVector, Ability ability) : base(unit)
        {
            this.ability = ability;
            this.targetVector = targetVector;
            this.originCoordinate = unit.Coordinate;
            this.unit = unit;
        }

        public override void Queue() {}

        public override void Execute()
        {
            ability.Use(unit, originCoordinate, targetVector);
        }

        public override void Undo()
        {
            ability.Undo(unit, originCoordinate, targetVector);
        }
    }
}
