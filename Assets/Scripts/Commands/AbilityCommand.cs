using System;
using System.Collections.Generic;
using Abilities;
using Commands.Shapes;
using GridObjects;
using Managers;
using Units;
using UnityEngine;

namespace Commands
{
    public class AbilityCommand : Command
    {
        private readonly Ability ability;
        private readonly Vector2 targetVector;
        private readonly Vector2Int originCoordinate;

        public AbilityCommand(IUnit unit, Vector2 targetVector, Ability ability) : base(unit)
        {
            this.ability = ability;
            this.targetVector = targetVector;
            this.originCoordinate = unit.Coordinate;
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
