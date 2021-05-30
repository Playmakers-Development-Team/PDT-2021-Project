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
        private Ability ability;
        private Vector2 targetVector;

        public AbilityCommand(IUnit unit, Vector2 targetVector, Ability ability) : base(unit)
        {
            this.ability = ability;
            this.targetVector = targetVector;
        }

        public override void Queue() {}

        public override void Execute()
        {
            ability.Use(unit, unit.Coordinate, targetVector);
        }

        public override void Undo()
        {
            
        }
    }
}
