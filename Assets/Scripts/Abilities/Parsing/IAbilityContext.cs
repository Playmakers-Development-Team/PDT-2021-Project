using System.Collections.Generic;
using Abilities.Shapes;
using UnityEngine;

namespace Abilities.Parsing
{
    public interface IAbilityContext
    {
        IAbilityUser OriginalUser { get; }

        IAbilityUser GetCachedUser(IAbilityUser realUser);

        IEnumerable<IAbilityUser> GetCachedUsersFromShape(Vector2Int originCoordinate, 
                                                          Vector2 directionVector, IShape shape);
    }
}
