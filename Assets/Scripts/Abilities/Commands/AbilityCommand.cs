using Commands;
using UnityEngine;

namespace Abilities.Commands
{
    public class AbilityCommand : Command
    {
        private readonly Ability ability;
        public Vector2 TargetVector { get; }
        public Vector2Int OriginCoordinate { get; }
        public IAbilityUser AbilityUser { get; }
        
        public AbilityCommand(IAbilityUser abilityUser, Vector2 targetVector, Ability ability)
        {
            this.ability = ability;
            TargetVector = targetVector;
            OriginCoordinate = abilityUser.Coordinate;
            AbilityUser = abilityUser;
        }

        public override void Execute()
        {
            ability.Use(AbilityUser, OriginCoordinate, TargetVector);
        }

        // public override void Undo()
        // {
        //     ability.Undo(AbilityUser, OriginCoordinate, TargetVector);
        // }
    }
}
