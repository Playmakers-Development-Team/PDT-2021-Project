using System;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public abstract class Conditional
    {
        [SerializeField, HideInInspector] protected string name;
        [SerializeField] protected AffectType affectType;
        
        protected IAbilityUser GetAffectedUser(IAbilityUser user, IAbilityUser target) => 
            affectType == AffectType.Target ? target : user;
    }
}
