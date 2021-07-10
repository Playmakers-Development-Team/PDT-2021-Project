using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public abstract class Conditional
    {
        [SerializeField, HideInInspector] protected string name;
        [SerializeField] protected AffectType affectType;
        
        protected IUnit GetAffectedUnit(IUnit user, IUnit target) => 
            affectType == AffectType.Target ? target : user;
    }
}
