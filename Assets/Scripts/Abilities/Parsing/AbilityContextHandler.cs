using System.Collections.Generic;
using System.Linq;
using Abilities.Shapes;
using UnityEngine;

namespace Abilities.Parsing
{
    public class AbilityContextHandler : IAbilityContext
    {
        private readonly Dictionary<IAbilityUser, IVirtualAbilityUser> virtualUsers = 
            new Dictionary<IAbilityUser, IVirtualAbilityUser>();
        
        public IAbilityUser OriginalUser { get; }

        public AbilityContextHandler(IAbilityUser originalUser) => OriginalUser = originalUser;

        public AbilityContextHandler(IAbilityUser originalUser, IEnumerable<IVirtualAbilityUser> existingVirtualUsers) 
            : this(originalUser)
        {
            foreach (IVirtualAbilityUser virtualUser in existingVirtualUsers)
                virtualUsers[virtualUser.RealAbilityUser] = virtualUser;
        }

        public IAbilityUser GetCachedUser(IAbilityUser realUser)
        {
            if (!virtualUsers.ContainsKey(realUser))
            {
                IVirtualAbilityUser virtualAbilityUser = realUser.CreateVirtualAbilityUser();
                virtualUsers[realUser] = virtualAbilityUser;
            }

            return virtualUsers[realUser];
        }

        public IEnumerable<IAbilityUser> GetCachedUsersFromShape(Vector2Int originCoordinate, 
                                                         Vector2 directionVector, IShape shape) =>
            shape.GetTargets(originCoordinate, directionVector)
                .OfType<IAbilityUser>()
                .Select(GetCachedUser);

        public void ApplyChanges()
        {
            foreach (IVirtualAbilityUser virtualAbilityUser in virtualUsers.Values)
                virtualAbilityUser.ApplyChanges();
        }
    }
}
