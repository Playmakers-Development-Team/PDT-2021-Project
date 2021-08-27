using System;
using Abilities.Shapes;
using Commands;
using UnityEngine;

namespace Abilities.Commands
{
    public class AbilityCommand : Command
    {
        public  Ability Ability { get; private set; }
        public ShapeDirection ShapeDirection { get; }
        public Vector2 TargetVector => ShapeDirection.IsometricVector;
        public Vector2Int OriginCoordinate { get; }
        public IAbilityUser AbilityUser { get; }
        
        [Obsolete("Use the constructor with ShapeDirection")]
        public AbilityCommand(IAbilityUser abilityUser, Vector2 targetVector, Ability ability)
        {
            Ability = ability;
            ShapeDirection = ShapeDirection.FromIsometric(targetVector);
            OriginCoordinate = abilityUser.Coordinate;
            AbilityUser = abilityUser;
        }
        
        public AbilityCommand(IAbilityUser abilityUser, ShapeDirection direction, Ability ability)
        {
            Ability = ability;
            ShapeDirection = direction;
            OriginCoordinate = abilityUser.Coordinate;
            AbilityUser = abilityUser;
        }

        public override void Execute()
        {
            Ability.Use(AbilityUser, OriginCoordinate, ShapeDirection);
        }

        // public override void Undo()
        // {
        //     ability.Undo(AbilityUser, OriginCoordinate, ShapeDirection);
        // }
    }
}
